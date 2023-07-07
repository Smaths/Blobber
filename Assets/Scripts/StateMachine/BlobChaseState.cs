using UnityEngine;

namespace StateMachine
{
    public class BlobChaseState : BlobBaseState
    {
        private Collider[] _cachedSearchColliders;
        private const int MaxColliders = 10;

        // Constructor
        public BlobChaseState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
            context.Blob.NavMeshAgent.speed = context.Blob.Speed;
        }

        public override void UpdateState()
        {
            SearchForPlayer();
            context.Blob.NavMeshAgent.SetDestination(context.Blob.PlayerBlob.transform.position);
        }

        public override void OnTriggerEnter(Collider other)
        {
            context.SwitchState(BlobState.Dead);
        }

        public override void ExitState() { }
        #endregion

        #region Search Logic
        private void SearchForPlayer()
        {
            _cachedSearchColliders ??= new Collider[MaxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(context.transform.position, context.Blob.SightRange, _cachedSearchColliders, context.Blob.SightMask);
            if (numColliders <= 0) // No target found
                context.SwitchState(BlobState.Patrol);
        }
        #endregion
    }
}