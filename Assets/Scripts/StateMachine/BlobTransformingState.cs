using UnityEngine;

namespace StateMachine
{
    public class BlobTransformingState : BlobBaseState
    {
        public BlobTransformingState(BlobStateManager context) : base(context) { }
        public override void EnterState() { }

        public override void UpdateState() { }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void OnCollisionEnter(Collision other) { }
    }
}