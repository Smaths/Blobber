using UnityEngine;

namespace StateMachine
{
    public class BlobPausedState : BlobBaseState
    {
        // Constructor
        public BlobPausedState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
            // Stop along current path
            context.Blob.NavMeshAgent.isStopped = true;
        }

        public override void UpdateState() { }

        public override void OnTriggerEnter(Collider other) { }

        public override void ExitState()
        {
            // Continue along current path
            context.Blob.NavMeshAgent.isStopped = false;
        }
        #endregion
    }
}