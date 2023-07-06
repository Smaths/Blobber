using System;
using Blobs;
using Sirenix.OdinInspector;
using UnityEngine;

public enum BlobState { Idle, Patrol, Chase, Paused, Dead }

namespace StateMachine
{
    [RequireComponent(typeof(Blob))]
    public class BlobStateManager : MonoBehaviour
    {
        [ShowInInspector] private BlobBaseState _currentState;
        private BlobIdleState _idleState;
        private BlobPatrolState _patrolState;
        private BlobChaseState _chaseState;
        private BlobTransformingState _transformingState;
        private BlobDeadState _deadState;

        public Blob Blob { get; private set; }

        #region Lifecycle
        private void Awake()
        {
            Blob ??= GetComponent<Blob>();

            _idleState = new BlobIdleState(this);
            _patrolState = new BlobPatrolState(this);
            _chaseState = new BlobChaseState(this);
            _transformingState = new BlobTransformingState(this);
            _deadState = new BlobDeadState(this);
        }

        private void Start()
        {
            // Initial state
            _currentState = _patrolState;
            _currentState.EnterState();
        }

        private void Update()
        {
            _currentState.UpdateState();
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdateState();
        }
        #endregion

        private void OnCollisionEnter(Collision other)
        {
            _currentState.OnCollisionEnter(other);
        }

        public void SwitchState(BlobState newState)
        {
            _currentState.ExitState();

            _currentState = newState switch
            {
                BlobState.Idle => _idleState,
                BlobState.Patrol => _patrolState,
                BlobState.Chase => _chaseState,
                BlobState.Paused => _idleState,
                BlobState.Dead => _deadState,
                _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
            };

            _currentState.EnterState();
        }
    }
}