using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum BlobType { Good, Bad }
public enum BlobAIState { Idle, Wander, Attack, Paused, Dead }

[RequireComponent(typeof(NavMeshAgent))]
public class BlobAI : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SceneObjectsOnly]
    [SerializeField] private GameObject _playerBlob;

    [Title("Blob AI", "Brains for blobs characters good and bad.", TitleAlignments.Split)]
    [SerializeField] private BlobType _blobType;
    [SerializeField] private BlobAIState _currentState;
    private BlobAIState _previousState;
    [SerializeField] private int _pointValue = 5;
    [SerializeField] private float _speed = 4f;
    private NavMeshAgent _agent;

    [Header("Transformation")]
    [SuffixLabel("second(s)"), HideIf("_blobType", BlobType.Bad)]
    [Tooltip("The duration the character stays in the untransformed state.")]
    [SerializeField] private Vector2 _untransformedDuration = new (5f, 6f);
    [SuffixLabel("second(s)"), HideIf("_blobType", BlobType.Bad)]
    [Tooltip("The duration the character stays in the transformed state.")]
    [SerializeField] private Vector2 _transformedDuration = new(3f, 4f);
    [SuffixLabel("second(s)"), HideIf("_blobType", BlobType.Bad)]
    [Tooltip("Rate at which the character transforms from one state to the other.")]
    [SerializeField] private float _transformationTime = 1.5f;
    private float _transformationTimeTrigger;

    [Header("Sight")]
    [Tooltip("Rate in seconds that the character will scan for targets that are on the 'Sight Mask' layer.")]
    [SuffixLabel("second(s)")] [MinValue(0.01f)]
    [SerializeField] private float _searchRate = 0.1f;
    [Tooltip("How far the blob can see.")]
    [SuffixLabel("meter(s)")] [MinValue(0.01f)]
    [SerializeField] private float _sightRange = 4f;
    [SerializeField] private LayerMask _sightMask;
    private bool _hadTarget;

    [Header("Wander")]
    [SuffixLabel("meter(s)"), MinValue(0)]
    [SerializeField] private float _wanderRange = 4f;
    [LabelText("Distance Threshold")]
    [SuffixLabel("meter(s)"), MinValue(0.01f)]
    [Tooltip("Range the character can be from random wander position before searching for a new one. Lower numbers are strict, higher numbers are more accommodating.")]
    [SerializeField] private float _wanderDistanceThreshold = 0.25f;
    [SuffixLabel("second(s)")]
    [Tooltip("How long the character will wait before finding a new wander position.")]
    [SerializeField] private Vector2 _wanderWaitTime = new (2f, 4f);
    [Tooltip("Select ground layer to check if random wander point is on the walkable ground.")]
    [SerializeField] private LayerMask _groundMask;
    [Tooltip("Select layers for the character to avoid when finding a random wander position.")]
    [SerializeField] private LayerMask _avoidMask;
    private bool _hasWanderPoint;
    private bool _isWaitingAtWanderPoint;

    [Header("Meshes")]
    [Required, ChildGameObjectsOnly]
    [SerializeField] private GameObject _blobMesh;
    [SerializeField] private GameObject _hat;

    [Header("Visuals")]
    [SerializeField] private Color _goodBlobColor = new (0.749f, 0.753f, 0.247f, 1.0f);
    [SerializeField] private Color _badBlobColor = new (0.682f, 0.298f, 0.294f, 1.0f);
    [AssetsOnly]
    [SerializeField] private Face _faceData;
    private Material _blobMaterial;
    private Material _faceMaterial;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    [Header("Info")]
    [SerializeField, DisplayAsString] private bool _isDestroying;
    [HorizontalGroup("Target")]
    [SerializeField, DisplayAsString] private bool _hasTarget;
    [HorizontalGroup("Target"), HideLabel]
    [SerializeField, DisplayAsString] private Vector3 _targetDestination;
    [HorizontalGroup("Transformation"), DisplayAsString]
    [SerializeField] private bool _isTransforming;
    [HorizontalGroup("Transformation"), DisplayAsString]
    [SerializeField] private bool _isTransformed;
    [SerializeField] private bool _showDebug;

    [FoldoutGroup("Events", false)]
    public UnityEvent<BlobAIState> OnBlobStateChange;
    [FoldoutGroup("Events")]
    public UnityEvent OnGainedTarget;
    [FoldoutGroup("Events")]
    public UnityEvent OnLostTarget;
    [FoldoutGroup("Events")]
    public UnityEvent OnIdle;
    [FoldoutGroup("Events")]
    public UnityEvent OnWander;
    [FoldoutGroup("Events")]
    public UnityEvent OnAttack;
    [FoldoutGroup("Events")]
    public UnityEvent OnTransform;
    [FoldoutGroup("Events")]
    public UnityEvent OnPause;
    [FoldoutGroup("Events")]
    public UnityEvent OnDeath;

    public BlobType Type => _blobType;

    #region Lifecycle
    private void OnDrawGizmosSelected()
    {
        // Draw the search radius gizmo in the Unity Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
    }

    private void OnValidate()
    {
        _agent ??= GetComponent<NavMeshAgent>();
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;

        // Update the blob's face
        if (_faceData != null && _faceMaterial)
            SetState(_currentState);
    }

    private void Awake()
    {
        _agent ??= GetComponent<NavMeshAgent>();
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;
        _blobMaterial ??= _blobMesh.GetComponent<Renderer>().materials[0];
        _faceMaterial ??= _blobMesh.GetComponent<Renderer>().materials[1];

        // Update the blob's face
        if (_faceData != null && _faceMaterial)
            SetState(_currentState);
    }

    private void OnEnable()
    {
        _agent.speed = _speed;

        if (_blobType == BlobType.Good)
        {
            _blobMaterial.color = _goodBlobColor;
            _hat.transform.localScale = Vector3.zero;

            // Set transformation time trigger
            _transformationTimeTrigger = Time.time + GetRandomDuration();
        }
    }

    private void Start()
    {
        switch (_blobType)
        {
            case BlobType.Good:
                break;
            case BlobType.Bad:
                StartCoroutine(SearchForPlayerCoroutine());
                break;
        }

        SetState(BlobAIState.Idle);
    }

    private void Update()
    {
        if (_isDestroying) return;  // Guard
        if (!_agent) return;
        if (_currentState == BlobAIState.Paused) return;

        if (_blobType == BlobType.Good)
            TransformationCheck();

        if (_isTransformed) return; // Prevent movement when good blob has transformed to bad.
        if (!_hasTarget)
            Wander();
    }
    #endregion

    #region Public Methods
    public void Disable()
    {
        if (!isActiveAndEnabled) return;
        
        _agent.speed = 0;
        SetState(BlobAIState.Paused);
    }

    public void Enable()
    {
        if (!isActiveAndEnabled) return;

        _agent.speed = _speed;
        SetState(_previousState);
    }
    #endregion

    private void SetState(BlobAIState newState)
    {
        if (newState == _currentState) return;  // Prevent redundant state sets

        // Set Face
        if (_faceMaterial & _faceData)
            UpdateFace(newState);

        _previousState = _currentState;
        _currentState = newState;

        // Events
        OnBlobStateChange?.Invoke(newState);

        switch (newState)
        {
            case BlobAIState.Idle:
                OnIdle?.Invoke();
                break;
            case BlobAIState.Wander:
                OnWander?.Invoke();
                break;
            case BlobAIState.Attack:
                OnAttack?.Invoke();
                break;
            case BlobAIState.Dead:
                PlayDeathAnimation();
                OnDeath?.Invoke();
                break;
            case BlobAIState.Paused:
                OnPause?.Invoke();
                break;
        }
    }

    #region Collider Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (_isDestroying) return;  // guard statement

        // Invert points if blob is transformed
        int points = _isTransformed ? -_pointValue : _pointValue;
        ScoreManager.instance.AddPoints(points, transform.position);

        if (BlobManager.instanceExists)
            BlobManager.Instance.OnBlobDestroy(this);

        SetState(BlobAIState.Dead);
    }

    private void PlayDeathAnimation()
    {
        // Animation
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f, vibrato: 0));
        sequence.Append(transform.DOScale(Vector3.zero, 0.2f));
        sequence.OnStart(() => _isDestroying = true);
        sequence.OnComplete(() => Destroy(gameObject, 0.1f));
        sequence.Play();
    }
    #endregion

    #region Search for Player
    private IEnumerator SearchForPlayerCoroutine()
    {
        while (!_isDestroying )
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _sightRange, _sightMask);

            if (colliders.Length == 0)
            {
                // No target found
                _hasTarget = false;

                SetState(BlobAIState.Wander);

                if (_hadTarget)
                {
                    _hadTarget = false;
                    OnLostTarget?.Invoke();
                }
            }
            else
            {
                // Target found
                _hasTarget = true;
                _hadTarget = true;
                _targetDestination = colliders[0].transform.position;

                _agent.SetDestination(_targetDestination);

                SetState(BlobAIState.Attack);

                OnGainedTarget?.Invoke();
            }

            yield return new WaitForSeconds(_searchRate);
        }
    }
    #endregion

    #region Wander
    private void Wander()
    {
        if (!_hasWanderPoint)
            SearchForWanderPoint();

        if (_hasWanderPoint)
            _agent.SetDestination(_targetDestination);

        Vector3 distance = transform.position - _targetDestination;
        if (_showDebug) print($"(distance to wander point: {distance.magnitude})");

        // Wander point reached
        if (distance.magnitude < _wanderDistanceThreshold && _isWaitingAtWanderPoint == false)
        {
            _isWaitingAtWanderPoint = true;

            float randomWaitTime = Random.Range(_wanderWaitTime.x, _wanderWaitTime.y);

            StartCoroutine(ResetWanderAfterDelayCoroutine(randomWaitTime));
        }
    }

    private IEnumerator ResetWanderAfterDelayCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        _isWaitingAtWanderPoint = false;
        _hasWanderPoint = false;
    }

    private void SearchForWanderPoint()
    {
        float randomX = Random.Range(-_wanderRange, _wanderRange);
        float randomZ = Random.Range(-_wanderRange, _wanderRange);

        Vector3 cachedPosition = transform.position;
        Vector3 randomPosition = new Vector3(cachedPosition.x + randomX, cachedPosition.y, cachedPosition.z + randomZ);

        // Check - random wander position is NOT inside another collider
        Collider[] colliders = Physics.OverlapBox(randomPosition, Vector3.one * 0.5f, Quaternion.identity, _avoidMask);
        if (colliders.Length != 0) return;

        // Check - random wander position is above the ground terrain
        if (!Physics.Raycast(randomPosition, Vector3.down, 2f, _groundMask)) return;

        if (_showDebug)
        {
            var debugObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugObject.transform.localScale = Vector3.one * 0.2f;
            Destroy(debugObject, 1f);

            Debug.DrawLine(transform.position, randomPosition, Color.red, 1f);
        }

        _hasWanderPoint = true;
        _targetDestination = randomPosition;
    }
    #endregion

    #region Face
    private void UpdateFace(BlobAIState state)
    {
        switch (state)
        {
            case BlobAIState.Idle:
                SetFace(_faceData.Idleface);
                break;
            case BlobAIState.Wander:
                SetFace(_faceData.WalkFace);
                break;
            case BlobAIState.Attack:
                SetFace(_faceData.attackFace);
                break;
            case BlobAIState.Dead:
                SetFace(_faceData.damageFace);
                break;
            case BlobAIState.Paused:
            default:
                break;
        }
    }
    private void SetFace(Texture tex)
    {
        _faceMaterial.SetTexture(MainTex, tex);
    }
    #endregion

    private void TransformationCheck()
    {
        if (_isTransforming) return;    // Ignore timer during transformation

        if (!(Time.time >= _transformationTimeTrigger)) return; // Timer still running
        // Timer complete

        // Start transformation FX - create new timer
        Sequence transformingSequence = GetTransformingSequence();
        transformingSequence.OnStart(OnTransformationDidStart);
        transformingSequence.OnComplete(OnTransformationDidComplete);
    }

    private Sequence GetTransformingSequence()
    {
        const float finalTransformationTime = 0.4f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOShakeRotation(_transformationTime, 45f, 4, 45));

        if (_isTransformed)
        {
            sequence.Append(_blobMaterial.DOColor(_goodBlobColor, _transformationTime));
            sequence.Join(_hat.transform.DOScale(0, _transformationTime));
        }
        else
        {
            sequence.Append(_blobMaterial.DOColor(_badBlobColor, _transformationTime));
            sequence.Join(_hat.transform.DOScale(0.8f, _transformationTime));
        }

        return sequence;
    }

    private void OnTransformationDidStart()
    {
        _isTransforming = true;

        OnTransform?.Invoke();

        Disable();
    }

    private void OnTransformationDidComplete()
    {
        _isTransforming = false;
        _isTransformed = !_isTransformed;

        float randomDuration = GetRandomDuration();

        _transformationTimeTrigger = Time.time + randomDuration;

        Enable();
    }

    private float GetRandomDuration()
    {
        float randomMin = _isTransformed ? _transformedDuration.x : _untransformedDuration.x;
        float randomMax = _isTransformed ? _transformedDuration.y : _untransformedDuration.y;
        float randomDuration = Random.Range(randomMin, randomMax);
        return randomDuration;
    }
}