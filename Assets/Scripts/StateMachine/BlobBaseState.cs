using UnityEngine;

namespace StateMachine
{
    public abstract class BlobBaseState
    {
        protected BlobStateManager context;

        // Constructor
        protected BlobBaseState(BlobStateManager context)
        {
            this.context = context;
        }

        // Abstract methods
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void OnTriggerEnter(Collider other);
        public abstract void ExitState();
    }
}