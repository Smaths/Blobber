using System;
using System.Collections;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Blobs
{
    public enum BlobType { Good, Bad }
    public enum BlobState { Idle, Patrol, Attack, Paused, Dead }

    [RequireComponent(typeof(NavMeshAgent))]
    public class Blob : MonoBehaviour
    {
        #region Fields
        private const float AnimationTime = 0.3f;
        private const int MaxColliders = 6;
        [BoxGroup("Dependencies")]
        [SceneObjectsOnly]
        [SerializeField] private GameObject _playerBlob;

        [Title("Blob AI", "Brains for blobs characters good and bad.", TitleAlignments.Split)]
        [SerializeField] private BlobType _blobType;
        [SerializeField] private BlobState _currentState;
        private BlobState _previousState;
        [SerializeField] private float _speed = 4f;

        [Header("Transformation")]
        [SuffixLabel("second(s)")] [HideIf("_blobType", BlobType.Bad)]
        [Tooltip("The duration the character stays in the untransformed state.")]
        [SerializeField] private Vector2 _untransformedDuration = new(5f, 6f);
        [SuffixLabel("second(s)")] [HideIf("_blobType", BlobType.Bad)]
        [Tooltip("The duration the character stays in the transformed state.")]
        [SerializeField] private Vector2 _transformedDuration = new(3f, 4f);
        [SuffixLabel("second(s)")] [HideIf("_blobType", BlobType.Bad)]
        [Tooltip("Rate at which the character transforms from one state to the other.")]
        [SerializeField] private float _transformationTime = 1.5f;
        private float _transformationTimeTrigger;

        [Header("Masks")]
        [Tooltip("Select ground layer to check if random wander point is on the walkable ground.")]
        [SerializeField] private LayerMask _groundMask;
        [Tooltip("Select layers for the character to avoid when finding a random wander position.")]
        [SerializeField] private LayerMask _avoidMask;

        [Header("Sight")]
        [Tooltip("Rate in seconds that the character will scan for targets that are on the 'Sight Mask' layer.")]
        [SuffixLabel("second(s)")] [MinValue(0.01f)]
        [SerializeField] private float _searchRate = 0.1f;
        [Tooltip("How far the blob can see.")]
        [SuffixLabel("meter(s)")] [MinValue(0.01f)]
        [SerializeField] private float _sightRange = 4f;
        [SerializeField] private LayerMask _sightMask;
        private bool _hadTarget;

        [Header("Patrol")]
        [SuffixLabel("meter(s)")] [MinValue(0)]
        [SerializeField] private float _searchRadius = 4f;
        [LabelText("Distance Threshold")]
        [SuffixLabel("meter(s)")] [MinValue(0.01f)]
        [Tooltip("Range the character can be from randomly selected target position before searching for a new one. Lower numbers are strict, higher numbers are more accommodating.")]
        [SerializeField] private float _patrolDistanceThreshold = 0.25f;
        [SuffixLabel("second(s)")] [MinValue(0)]
        [Tooltip("How long the character will wait before finding a new position.")]
        [SerializeField] private Vector2 _patrolWaitTime = new(2f, 4f);
        private bool _hasPatrolPoint;
        private bool _isWaitingAtPatrolPoint;

        [Header("Meshes")]
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private GameObject _blobMesh;
        [SerializeField] private GameObject _hat;

        [Header("Visuals")]
        [SerializeField] private Color _goodBlobColor = new(0.749f, 0.753f, 0.247f, 1.0f);
        [SerializeField] private Color _badBlobColor = new(0.682f, 0.298f, 0.294f, 1.0f);
        [AssetsOnly]
        [SerializeField] private global::Face _faceData;
        private Material _blobMaterial;
        private Material _faceMaterial;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        [ChildGameObjectsOnly]
        [SerializeField] private GameObject _deathFX;
        private ParticleSystem _deathPS;

        [Header("Info")]
        [SerializeField] [DisplayAsString] private bool _isDestroying;
        [HorizontalGroup("Target")]
        [SerializeField] [DisplayAsString] private bool _hasTarget;
        [HorizontalGroup("Target")] [HideLabel]
        [SerializeField] [DisplayAsString] private Vector3 _targetDestination;
        [HorizontalGroup("Transformation")] [DisplayAsString]
        [SerializeField] private bool _isTransforming;
        [HorizontalGroup("Transformation")] [DisplayAsString]
        [SerializeField] private bool _isTransformed;

        [Space] [PropertyOrder(100)]
        [SerializeField] private bool _showDebug;

        private NavMeshAgent _agent;
        private Collider[] _searchColliders;
        private Collider[] _cachedPatrolColliders;
        private Vector3 _originalScale;
        #endregion

        #region Public Properties
        public BlobType BlobType => _blobType;
        public bool IsTransformed => _isTransformed;
        #endregion

        #region Events
        [FoldoutGroup("Events", false)] public UnityEvent<BlobState> OnBlobStateChange;
        [FoldoutGroup("Events")] public UnityEvent OnGainedTarget;
        [FoldoutGroup("Events")] public UnityEvent OnLostTarget;
        [FoldoutGroup("Events")] public UnityEvent OnIdle;
        [FoldoutGroup("Events")] public UnityEvent OnWander;
        [FoldoutGroup("Events")] public UnityEvent OnAttack;
        [FoldoutGroup("Events")] public UnityEvent OnTransform;
        [FoldoutGroup("Events")] public UnityEvent OnPause;
        [FoldoutGroup("Events")] public UnityEvent OnDeath;
        #endregion

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
            _originalScale = transform.localScale;

            // Update the blob's face
            if (_faceData != null && _faceMaterial)
                SetState(_currentState);
        }

        private void OnEnable()
        {
            _agent.speed = _speed;
            _isDestroying = false;
            _isTransforming = false;

            if (_blobType == BlobType.Good)
            {
                _blobMaterial.color = _goodBlobColor;
                _hat.transform.localScale = Vector3.zero;

                // Set initial transformation time trigger (extra randomness)
                _transformationTimeTrigger = Time.time + GetTransformationDuration() + Random.Range(-1f, 3f);
            }
        }

        private void OnDisable()
        {
            // Cleanup Death FX
            _deathFX.gameObject.SetActive(false);
            transform.localScale = _originalScale;
        }

        private void Start()
        {
            _deathPS = _deathFX.GetComponent<ParticleSystem>();

            if (_playerBlob == null) _playerBlob = FindObjectOfType<PlayerController>().gameObject;

            switch (_blobType)
            {
                case BlobType.Good:
                    break;
                case BlobType.Bad:
                    StartCoroutine(SearchForPlayerCoroutine());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetState(BlobState.Idle);
        }

        private void Update()
        {
            if (_isDestroying) return; // Guard
            if (!_agent) return;
            if (_currentState == BlobState.Paused) return;

            if (_blobType == BlobType.Good)
                TransformationCheck();

            if (_isTransformed) return; // Prevent movement when good blob has transformed to bad.
            if (!_hasTarget)
                Patrol();
        }
        #endregion

        #region Public Methods
        public void Disable()
        {
            if (!isActiveAndEnabled) return;

            _agent.speed = 0;
            SetState(BlobState.Paused);
        }

        public void Enable()
        {
            if (!isActiveAndEnabled) return;

            _agent.speed = _speed;
            SetState(_previousState);
        }
        #endregion

        #region State
        private void SetState(BlobState newState)
        {
            if (newState == _currentState) return; // Prevent redundant state sets

            // Set Face
            if (_faceMaterial & _faceData)
                UpdateFace(newState);

            _previousState = _currentState;
            _currentState = newState;

            // Events
            OnBlobStateChange?.Invoke(newState);

            switch (newState)
            {
                case BlobState.Idle:
                    OnIdle?.Invoke();
                    break;
                case BlobState.Patrol:
                    OnWander?.Invoke();
                    break;
                case BlobState.Attack:
                    OnAttack?.Invoke();
                    break;
                case BlobState.Dead:
                    OnDeath?.Invoke();
                    break;
                case BlobState.Paused:
                    OnPause?.Invoke();
                    break;
            }
        }
        #endregion

        #region Collider Trigger
        private void OnTriggerEnter(Collider other)
        {
            if (_isDestroying) return; // guard statement
            _isDestroying = true;

            ScoreManager.instance.AddPoints(this);

            SetState(BlobState.Dead);

            DoDeath();
        }
        #endregion

        #region Death
        private void DoDeath()
        {
            // Create mesh animation sequence
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), AnimationTime, 0));
            sequence.Append(transform.DOScale(Vector3.zero, AnimationTime));

            // Determine death fx duration
            float deathDuration = _deathPS.main.duration >= sequence.Duration()
                ? _deathPS.main.duration
                : sequence.Duration();

            // Play death FX
            sequence.Play();
            if (_blobType == BlobType.Bad || _isTransformed)
                _deathFX.gameObject.SetActive(true);

            // Return to pool after death FX are complete
            StartCoroutine(ReturnToPoolAfterDelay(deathDuration + 0.01f));
        }

        private IEnumerator ReturnToPoolAfterDelay(float time)
        {
            yield return new WaitForSeconds(time);

            if (!BlobManager.instanceExists)
            {
                Debug.LogWarning($"{nameof(BlobManager)} singleton doesn't exist, unable to return {name} to object pool.");
                yield break;
            }

            BlobManager.Instance.ReturnToPool(this);
        }
        #endregion

        #region Search
        private IEnumerator SearchForPlayerCoroutine()
        {
            while (!_isDestroying)
            {
                _searchColliders ??= new Collider[MaxColliders];
                int numColliders =
                    Physics.OverlapSphereNonAlloc(transform.position, _sightRange, _searchColliders, _sightMask);

                if (numColliders == 0)
                {
                    // No target found
                    _hasTarget = false;

                    SetState(BlobState.Patrol);

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
                    _targetDestination = _searchColliders[0].transform.position;

                    _agent.SetDestination(_targetDestination);

                    SetState(BlobState.Attack);

                    OnGainedTarget?.Invoke();
                }

                yield return new WaitForSeconds(_searchRate);
            }
        }
        #endregion

        #region Patrol
        private void Patrol()
        {
            if (!_hasPatrolPoint)
                SearchForPatrolPoint();

            if (_hasPatrolPoint)
                _agent.SetDestination(_targetDestination);

            Vector3 distance = transform.position - _targetDestination;
            if (_showDebug) print($"(distance to wander point: {distance.magnitude})");

            // Wander point reached
            if (distance.magnitude < _patrolDistanceThreshold && _isWaitingAtPatrolPoint == false)
            {
                _isWaitingAtPatrolPoint = true;

                float randomWaitTime = Random.Range(_patrolWaitTime.x, _patrolWaitTime.y);

                StartCoroutine(ResetPatrolAfterDelayCoroutine(randomWaitTime));
            }
        }

        private IEnumerator ResetPatrolAfterDelayCoroutine(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            _isWaitingAtPatrolPoint = false;
            _hasPatrolPoint = false;
        }

        // ---
        // NOTE: Not working, but would be great if it did.
        private void GetNewPatrolPoint()
        {
            int walkableLayerMask = NavMesh.GetAreaFromName("Walkable"); // Replace "Walkable" with the actual name of your walkable NavMesh layer
            Vector3 randomPosition = RandomNavSphere(transform.position, _searchRadius, walkableLayerMask);

            print($"{gameObject.name} - Random Position: {randomPosition}");
            
            _hasPatrolPoint = true;
            _targetDestination = randomPosition;
        }

        private Vector3 RandomNavSphere(Vector3 origin, float radius, int layerMask)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;

            NavMesh.SamplePosition(new Vector3(randomDirection.x, origin.y, randomDirection.z), out NavMeshHit navHit, radius, layerMask);

            return navHit.position;
        }
        // ---

        private void SearchForPatrolPoint()
        {
            float randomX = Random.Range(-_searchRadius, _searchRadius);
            float randomZ = Random.Range(-_searchRadius, _searchRadius);

            Vector3 cachedPosition = transform.position;
            var randomPosition = new Vector3(cachedPosition.x + randomX, cachedPosition.y, cachedPosition.z + randomZ);

            // Check - random position is NOT inside another collider
            _cachedPatrolColliders ??= new Collider[MaxColliders];
            int numColliders = Physics.OverlapBoxNonAlloc(randomPosition, Vector3.one * 0.5f, _cachedPatrolColliders, Quaternion.identity, _avoidMask);
            if (numColliders > 0)
            {
                return;
            }

            // Check - random position is above the ground terrain
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
        private void UpdateFace(BlobState state)
        {
            switch (state)
            {
                case BlobState.Idle:
                    SetFace(_faceData.Idleface);
                    break;
                case BlobState.Patrol:
                    SetFace(_faceData.WalkFace);
                    break;
                case BlobState.Attack:
                    SetFace(_faceData.attackFace);
                    break;
                case BlobState.Dead:
                    SetFace(_faceData.damageFace);
                    break;
                case BlobState.Paused:
                default:
                    break;
            }
        }

        private void SetFace(Texture tex)
        {
            _faceMaterial.SetTexture(MainTex, tex);
        }
        #endregion

        #region Transformation
        private void TransformationCheck()
        {
            if (_isTransforming) return; // Ignore timer during transformation

            if (!(Time.time >= _transformationTimeTrigger)) return; // Timer still running
            // Timer complete

            // Start transformation FX - create new timer
            Sequence transformingSequence = GetTransformingSequence();
            transformingSequence.OnStart(OnTransformationDidStart);
            transformingSequence.OnComplete(OnTransformationDidComplete);
        }

        private Sequence GetTransformingSequence()
        {
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

            float randomDuration = GetTransformationDuration();

            _transformationTimeTrigger = Time.time + randomDuration;

            Enable();
        }

        private float GetTransformationDuration()
        {
            float randomMin = _isTransformed ? _transformedDuration.x : _untransformedDuration.x;
            float randomMax = _isTransformed ? _transformedDuration.y : _untransformedDuration.y;
            float randomDuration = Random.Range(randomMin, randomMax);
            return randomDuration;
        }
        #endregion
    }
}