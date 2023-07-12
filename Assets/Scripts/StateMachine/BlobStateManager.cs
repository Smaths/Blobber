using System;
using System.Collections;
using Blobs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum BlobState { Idle, Wander, Patrol, Chase, Paused, Transforming, Dead }

namespace StateMachine
{
    [RequireComponent(typeof(Blob))]
    public class BlobStateManager : MonoBehaviour
    {
        [ShowInInspector] private BlobBaseState _currentState;
        private Blob _blob;
        private Transform _blobTransform;
        private BlobBaseState _previousState;
        private BlobState _initialState;

        // Concrete States
        private BlobIdleState _idleState;
        private BlobWanderState _wanderState;
        private BlobPatrolState _patrolState;
        private BlobChaseState _chaseState;
        private BlobTransformingState _transformingState;
        private BlobPausedState _pausedState;
        private BlobDeadState _deadState;

        private float _transformTimer;
        private IEnumerator _transformCoroutine;
        private bool _shouldTransform;

        #region Public Properties
        public Blob Blob => _blob;
        public bool IsTransformed { get; private set; }
        public Transform BlobTransform => _blobTransform;
        #endregion

        #region Events
        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent<BlobState> OnStateChanged;
        #endregion

        #region Lifecycle
        private void Awake()
        {
            // Blob settings
            _blob ??= GetComponent<Blob>();
            _blobTransform = _blob.BlobTransform;
            _initialState = _blob.InitialState;
            _shouldTransform = _blob.BlobType == BlobType.Good;

            // Initialize concrete states
            _idleState = new BlobIdleState(this);
            _wanderState = new BlobWanderState(this);
            _patrolState = new BlobPatrolState(this);
            _chaseState = new BlobChaseState(this);
            _transformingState = new BlobTransformingState(this);
            _pausedState = new BlobPausedState(this);
            _deadState = new BlobDeadState(this);
        }

        private void OnEnable()
        {
            // Initial state
            _currentState = _previousState = Blob.BlobType switch
            {
                BlobType.Good => _wanderState,
                BlobType.Bad => _patrolState,
                _ => throw new ArgumentOutOfRangeException()
            };

            _currentState.EnterState();

            InvokeEvents(_currentState);

            // Transformation
            IsTransformed = false; // Reset transformed state
            if (_shouldTransform) ResetTransformationTimer(Random.Range(0, 3f));

            // DeathFX
            Blob.DeathFX.gameObject.SetActive(false);
        }

        private void Update()
        {
            _currentState.UpdateState();

            if (TransformationTimerCheck())
                SwitchState(BlobState.Transforming);
        }
        #endregion

        #region Trigger Event
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                _currentState.OnTriggerEnter(other);
        }
        #endregion

        #region Public Methods
        public void SwitchState(BlobState newState)
        {
            _currentState.ExitState();

            SetPreviousState(_currentState);

            _currentState = newState switch
            {
                BlobState.Idle => _idleState,
                BlobState.Wander => _wanderState,
                BlobState.Patrol => _patrolState,
                BlobState.Chase => _chaseState,
                BlobState.Transforming => _transformingState,
                BlobState.Paused => _pausedState,
                BlobState.Dead => _deadState,
                _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
            };

            _currentState.EnterState();

            InvokeEvents(_currentState);
        }

        public void ReturnToPreviousState()
        {
            if (_previousState == null)
            {
                SwitchState(_initialState);
                return;
            }

            BlobState previousState = _previousState switch
            {
                BlobIdleState => BlobState.Idle,
                BlobWanderState => BlobState.Wander,
                BlobPatrolState => BlobState.Patrol,
                BlobChaseState => BlobState.Chase,
                BlobTransformingState => BlobState.Transforming,
                BlobPausedState => BlobState.Paused,
                BlobDeadState => BlobState.Dead,
                _ => throw new ArgumentOutOfRangeException(nameof(_previousState))
            };

            SwitchState(previousState);
        }
        #endregion

        #region Transformation
        public void SetTransformed(bool isTransformed)
        {
            IsTransformed = isTransformed;
        }

        private void ResetTransformationTimer(float extraTime)
        {
            _transformTimer = Time.time + Blob.GetTransformationInterval(IsTransformed) + Blob.TransformationDuration + extraTime;
        }

        private void ResetTransformationTimer()
        {
            _transformTimer = Time.time + Blob.GetTransformationInterval(IsTransformed) + Blob.TransformationDuration;
        }

        private bool TransformationTimerCheck()
        {
            // Guards
            if (_shouldTransform == false) return false;
            if (_currentState == _transformingState) return false;
            if (_currentState == _deadState) return false;
            // Increment transformation timer while paused
            if (_currentState == _pausedState)
            {
                _transformTimer += Time.deltaTime;
                return false;
            }

            if (Time.time < _transformTimer) return false; // Timer running
            // Timer complete
            ResetTransformationTimer();
            return true;

        }
        #endregion

        #region Private Methods
        private void SetPreviousState(BlobBaseState currentState)
        {
            if (currentState != _transformingState && currentState != _pausedState)
                _previousState = currentState;
        }
        private void InvokeEvents(BlobBaseState state)
        {
            BlobState blobState = state switch
            {
                BlobChaseState => BlobState.Chase,
                BlobDeadState => BlobState.Dead,
                BlobIdleState => BlobState.Idle,
                BlobPatrolState => BlobState.Patrol,
                BlobPausedState => BlobState.Paused,
                BlobTransformingState => BlobState.Paused,
                BlobWanderState => BlobState.Wander,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };

            OnStateChanged?.Invoke(blobState);
        }
        #endregion
    }
}