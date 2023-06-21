using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum BlobType { Good, Bad }
public enum BlobAIState { Idle, Patrol, Attack, Dead }

public class BlobAI : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SerializeField] private NavMeshAgent _agent;
    [BoxGroup("Dependencies")]
    [SerializeField] private GameObject _playerBlob;
    [BoxGroup("Dependencies")]
    [SerializeField] private GameObject _blobMesh;

    [Title("Blob AI", "Brains for blobs characters good and bad." )]
    [SerializeField] private BlobType _blobType;
    [SerializeField] private int _pointValue = 5;
    [SerializeField] private BlobAIState _currentState;
    [Header("Sight")]
    [Tooltip("Rate in seconds that the character will scan for targets that are on the 'Sight Mask' layer.")]
    [SuffixLabel("second(s)")]
    [SerializeField] private float _searchRate = 0.25f;
    [FormerlySerializedAs("_sightRadius")]
    [Tooltip("How far the blob can see.")]
    [SuffixLabel("meter(s)")]
    [SerializeField] private float _sightRange = 4f;
    [SerializeField] private LayerMask _sightMask;

    [Header("Patrol")]
    [SuffixLabel("meter(s)")]
    [SerializeField] private float _patrolRange = 4f;
    [LabelText("Distance Threshold")]
    [Tooltip("Range the character can be from random patrol position before searching for a new one. Lower numbers are strict, higher numbers are more accommodating.")]
    [SuffixLabel("meter(s)")]
    [SerializeField] private float _patrolDistanceThreshold = 1f;
    [Tooltip("How long the character will wait before finding a new patrol position.")]
    [SuffixLabel("second(s)")]
    [SerializeField] private float _patrolWaitTime = 3f;
    [Tooltip("Select ground layer to check if random patrol point is on the walkable ground.")]
    [SerializeField] private LayerMask _groundMask;
    [Tooltip("Select layers for the character to avoid when finding a random patrol position.")]
    [SerializeField] private LayerMask _avoidMask;

    [FormerlySerializedAs("_faces")]
    [Header("Faces :D")]
    [SerializeField] private Face _faceData;
    [SerializeField] private Material _faceMaterial;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    [Header("Debugging")]
    [SerializeField] private bool _showDebug;

    private bool _isDestroying;
    private Vector3 _targetDestination;
    private bool _hasTarget;
    private bool _hasPatrolPoint;

    [FoldoutGroup("Events", false)]
    public UnityEvent<BlobAIState> OnBlobStateChange;
    [FoldoutGroup("Events")]
    public UnityEvent OnGainedTarget;
    [FoldoutGroup("Events")]
    public UnityEvent OnLostTarget;
    [FoldoutGroup("Events")]
    public UnityEvent OnIdle;
    [FoldoutGroup("Events")]
    public UnityEvent OnPatrol;
    [FoldoutGroup("Events")]
    public UnityEvent OnAttack;
    [FoldoutGroup("Events")]
    public UnityEvent OnDeath;
    private bool _hadTarget;

    public BlobAIState CurrentState
    {
        get => _currentState;
        private set
        {
            switch (value)
            {
                case BlobAIState.Idle:
                    OnIdle?.Invoke();
                    break;
                case BlobAIState.Patrol:
                    OnPatrol?.Invoke();
                    break;
                case BlobAIState.Attack:
                    OnAttack?.Invoke();
                    break;
                case BlobAIState.Dead:
                    OnDeath?.Invoke();
                    break;
            }

            if (_faceMaterial)
            {
                UpdateFade(value);
            }

            OnBlobStateChange?.Invoke(value);

            _currentState = value;
        }
    }

    #region Lifecycle
    private void OnDrawGizmosSelected()
    {
        // Draw the search radius gizmo in the Unity Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
    }

    private void OnValidate()
    {
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;

        if (_blobMesh)
        {
            if (_faceData != null)
            {
                if (_faceMaterial)
                    CurrentState = _currentState;
            }
        }
    }

    private void Awake()
    {
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;

        if (_blobMesh)
        {
            if (_faceData != null)
            {
                _faceMaterial = _blobMesh.GetComponent<Renderer>().materials[1];

                if (_faceMaterial)
                    CurrentState = _currentState;
            }
        }
    }

    private void Start()
    {
        if (_agent)
        {
            // StartCoroutine(SearchForPlayerCoroutine());
        }

        CurrentState = BlobAIState.Idle;
    }

    private void Update()
    {
        if (_isDestroying) return;  // Guard
        if (!_agent) return;

        if (!_hasTarget)
        {
            Patrol();
        }
    }
    #endregion

    #region Collider Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (_isDestroying) return;  // guard statement

        print(_pointValue > 0
            ? $"Player hit {gameObject.name} (+ {_pointValue})"
            : $"Player hit {gameObject.name} (- {_pointValue})");

        LevelManager.instance.AddPoints(_pointValue);

        CurrentState = BlobAIState.Dead;

        PlayDeathAnimation();
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

            if (colliders.IsNullOrEmpty())
            {
                // No Target
                _hasTarget = false;
                _targetDestination = transform.position;

                CurrentState = BlobAIState.Patrol;
                if (_hadTarget)
                {
                    _hadTarget = false;
                    OnLostTarget?.Invoke();
                }
            }
            else
            {
                // Set Target
                _hasTarget = true;
                _hadTarget = true;
                _targetDestination = colliders[0].transform.position;

                CurrentState = BlobAIState.Attack;
                OnGainedTarget?.Invoke();
            }

            _agent.SetDestination(_targetDestination);

            yield return new WaitForSeconds(_searchRate);
        }
    }
    #endregion

    #region Patrol
    private void Patrol()
    {
        if (!_hasPatrolPoint)
        {
            print($"{gameObject.name} - Search for Patrol");
            SearchForPatrolPoint();
        }

        if (_hasPatrolPoint)
        {
            print($"{gameObject.name} - Set Patrol Point {_targetDestination})");
            _agent.SetDestination(_targetDestination);
        }

        Vector3 distanceToPatrolPoint = transform.position - _targetDestination;

        print($"(distance: {distanceToPatrolPoint.magnitude})");
        // Patrol point reached

        if (distanceToPatrolPoint.magnitude < _patrolDistanceThreshold)
        {
            print($"{gameObject.name} - Patrol location reached (current distance: {distanceToPatrolPoint.magnitude})");

            _hasPatrolPoint = false;
        }
    }

    private void SearchForPatrolPoint()
    {
        float randomX = Random.Range(-_patrolRange, _patrolRange);
        float randomZ = Random.Range(-_patrolRange, _patrolRange);

        Vector3 cachedPosition = transform.position;
        Vector3 randomPosition = new Vector3(cachedPosition.x + randomX, cachedPosition.y, cachedPosition.z + randomZ);

        // Check - random patrol position is NOT inside another collider
        Collider[] colliders = Physics.OverlapBox(randomPosition, Vector3.one * 0.5f, Quaternion.identity, _avoidMask);
        if (colliders.Length != 0) return;

        // Check - random patrol position is above the ground terrain
        if (!Physics.Raycast(randomPosition, Vector3.down, 2f, _groundMask)) return;

        if (_showDebug)
        {
            var debugObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugObject.transform.localScale = Vector3.one * 0.2f;
            Destroy(debugObject, 1f);

            Debug.DrawLine(transform.position, randomPosition, Color.red, 1f);
        }

        _hasPatrolPoint = true;
        _targetDestination = randomPosition;
    }
    #endregion

    #region Face
    private void UpdateFade(BlobAIState state)
    {
        switch (state)
        {
            case BlobAIState.Idle:
                SetFace(_faceData.Idleface);
                break;
            case BlobAIState.Patrol:
                SetFace(_faceData.WalkFace);
                break;
            case BlobAIState.Attack:
                SetFace(_faceData.attackFace);
                break;
            case BlobAIState.Dead:
                SetFace(_faceData.damageFace);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    private void SetFace(Texture tex)
    {
        _faceMaterial.SetTexture(MainTex, tex);
    }
    #endregion
}