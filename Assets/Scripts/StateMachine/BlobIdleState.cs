using UnityEngine;

namespace StateMachine
{
    public class BlobIdleState : BlobBaseState
    {
        // Constructor
        public BlobIdleState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
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