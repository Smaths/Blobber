using System;
using System.Collections;
using Blobs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public enum BlobState { Idle, Wander, Patrol, Chase, Paused, Transforming, Dead }

namespace StateMachine
{
    [RequireComponent(typeof(Blob))]
    public class BlobStateManager : MonoBehaviour
    {
        [ShowInInspector] private BlobBaseState _currentState;
        private BlobBaseState _previousState;

        // Concrete States
        private BlobIdleState _idleState;
        private BlobWanderState _wanderState;
        private BlobPatrolState _patrolState;
        private BlobChaseState _chaseState;
        private BlobTransformingState _transformingState;
        private BlobDeadState _deadState;
        private BlobPausedState _pausedState;

        private float _transformTimer;
        private IEnumerator _transformCoroutine;

        #region Public Properties
        public Blob Blob { get; private set; }
        public bool IsTransformed { get; private set; }
        #endregion

        #region Events
        [Space]
        [FoldoutGroup("Events", false)] public UnityEvent<BlobState> OnStateChanged;
        [FoldoutGroup("Events")] public UnityEvent OnIdle;
        [FoldoutGroup("Events")] public UnityEvent OnWander;
        [FoldoutGroup("Events")] public UnityEvent OnPatrol;
        [FoldoutGroup("Events")] public UnityEvent OnChase;
        [FoldoutGroup("Events")] public UnityEvent OnPaused;
        [FoldoutGroup("Events")] public UnityEvent OnTransforming;
        [FoldoutGroup("Events")] public UnityEvent OnDead;
        #endregion

        #region Lifecycle
        private void Awake()
        {
            Blob ??= GetComponent<Blob>();

            // Initialize concrete states
            _idleState = new BlobIdleState(this);
            _wanderState = new BlobWanderState(this);
            _patrolState = new BlobPatrolState(this);
            _chaseState = new BlobChaseState(this);
            _transformingState = new BlobTransformingState(this);
            _deadState = new BlobDeadState(this);
            _pausedState = new BlobPausedState(this);
        }

        private void OnEnable()
        {
            ResetObject();

            // Transformation
            if (Blob.BlobType == BlobType.Good)
            {
                SetNewTransformationTimer();
            }

            // Initial state
            _currentState = _previousState = Blob.BlobType switch
            {
                BlobType.Good => _wanderState,
                BlobType.Bad => _patrolState,
                _ => throw new ArgumentOutOfRangeException()
            };

            _currentState.EnterState();

            InvokeEvents(_currentState);
        }

        private void OnDisable()
        {
            ResetObject();
        }

        private void ResetObject()
        {
            // Reset for object pooling
            transform.localScale = Vector3.one; // Reset scale
            Blob.DeathFX.gameObject.SetActive(false); // Disable explosion
            IsTransformed = false; // Reset transformed state

            if (Blob.BlobType == BlobType.Good)
            {
                Blob.Hat.transform.localScale = Vector3.zero;
                Blob.BlobMaterial.color = Blob.GoodBlobColor;
            }
        }

        private void Update()
        {
            _currentState.UpdateState();

            if (TransformationTimerCheck())
                SwitchState(BlobState.Transforming);
        }
        #endregion

        // Collider Events
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                _currentState.OnTriggerEnter(other);
        }

        // Public Methods
        public void SwitchState(BlobState newState)
        {
            _currentState.ExitState();

            if (_currentState != _transformingState)
            {
                _previousState = _currentState;
            }

            _currentState = newState switch
            {
                BlobState.Idle => _idleState,
                BlobState.Wander => _wanderState,
                BlobState.Patrol => _patrolState,
                BlobState.Chase => _chaseState,
                BlobState.Paused => _pausedState,
                BlobState.Transforming => _transformingState,
                BlobState.Dead => _deadState,
                _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
            };

            _currentState.EnterState();

            InvokeEvents(_currentState);
        }

        private void InvokeEvents(BlobBaseState state)
        {
            BlobState blobState;
            switch (state)
            {
                case BlobChaseState:
                    blobState = BlobState.Chase;
                    OnChase?.Invoke();
                    break;
                case BlobDeadState:
                    blobState = BlobState.Dead;
                    OnDead?.Invoke();
                    break;
                case BlobIdleState:
                    blobState = BlobState.Idle;
                    OnIdle?.Invoke();
                    break;
                case BlobPatrolState:
                    blobState = BlobState.Patrol;
                    OnPatrol?.Invoke();
                    break;
                case BlobPausedState:
                    blobState = BlobState.Paused;
                    OnPaused?.Invoke();
                    break;
                case BlobTransformingState:
                    blobState = BlobState.Paused;
                    OnTransforming?.Invoke();
                    break;
                case BlobWanderState:
                    blobState = BlobState.Wander;
                    OnWander?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }

            OnStateChanged?.Invoke(blobState);
        }

        public void ReturnToPreviousState()
        {
            if (_previousState == null)
            {
#if UNITY_EDITOR
                Debug.Log($"{name} tried to return to previous state but none are available.");
#endif
                return;
            }

            BlobState previousState = _previousState switch
            {
                BlobChaseState => BlobState.Chase,
                BlobDeadState => BlobState.Dead,
                BlobIdleState => BlobState.Idle,
                BlobPatrolState => BlobState.Patrol,
                BlobTransformingState => BlobState.Transforming,
                BlobWanderState => BlobState.Wander,
                _ => throw new ArgumentOutOfRangeException(nameof(_previousState))
            };

            SwitchState(previousState);
        }

        #region Transformation
        public void SetTransformed(bool isTransformed)
        {
            IsTransformed = isTransformed;

            if (!isTransformed)
                SetNewTransformationTimer();
        }

        private void SetNewTransformationTimer()
        {
            _transformTimer = Time.time + Blob.GetTransformationInterval(IsTransformed);
        }

        private bool TransformationTimerCheck()
        {
            // Guards
            if (_currentState == _transformingState) return false;
            if (_currentState == _deadState) return false;

            if (Time.time < _transformTimer) return false; // Timer running
            // Timer complete
            return true;
        }

        // private IEnumerator TransformCoroutine()
        // {
        //     while (isActiveAndEnabled)
        //     {
        //         yield return new WaitForSeconds(Blob.GetTransformationInterval(IsTransformed));;
        //
        //         // Don't switch if blob is dead or transforming
        //         if (_currentState == _deadState || _currentState == _transformingState) continue;
        //
        //         SwitchState(BlobState.Transforming);
        //     }
        // }
        //
        // public void RestartTransformationTimer()
        // {
        //     StopCoroutine(TransformCoroutine());
        //     StartCoroutine(TransformCoroutine());
        // }
        #endregion
    }
}