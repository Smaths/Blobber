using UnityEngine;

namespace StateMachine
{
    public class BlobIdleState : BlobBaseState
    {
        // Constructor
        public BlobIdleState(BlobStateManager context) : base(context) { }

        // Required State Methods
        public override void EnterState() { }

        public override void UpdateState() { }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void OnCollisionEnter(Collision other)
        {
            context.SwitchState(BlobState.Dead);
        }
    }
}