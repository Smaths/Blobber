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
            context.Blob.NavMeshAgent.speed = 0;
            context.Blob.NavMeshAgent.ResetPath();
        }

        public override void UpdateState() { }

        public override void OnTriggerEnter(Collider other)
        {
            context.SwitchState(BlobState.Dead);
        }

        public override void ExitState() { }
        #endregion
    }
}