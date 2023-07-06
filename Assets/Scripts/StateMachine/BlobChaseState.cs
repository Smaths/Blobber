using UnityEngine;

namespace StateMachine
{
    public class BlobChaseState : BlobBaseState
    {
        private Collider[] _cachedSearchColliders;
        private const int MaxColliders = 10;

        // Constructor
        public BlobChaseState(BlobStateManager context) : base(context) { }

        // Required State Methods
        public override void EnterState() { }

        public override void UpdateState()
        {
            SearchForPlayer();
            context.Blob.NavMeshAgent.SetDestination(context.Blob.PlayerBlob.transform.position);
        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void OnCollisionEnter(Collision other)
        {
            context.SwitchState(BlobState.Dead);
        }

        // Private Implementation
        private void SearchForPlayer()
        {
            _cachedSearchColliders ??= new Collider[MaxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(context.transform.position, context.Blob.SightRange,
                _cachedSearchColliders, context.Blob.SightMask);
            if (numColliders <= 0)
                // No target found
                context.SwitchState(BlobState.Patrol);
        }
    }
}