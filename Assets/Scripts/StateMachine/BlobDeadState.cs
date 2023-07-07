using Blobs;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace StateMachine
{
    public class BlobDeadState : BlobBaseState
    {
        private float _deathTimer;
        private Vector3 _originalScale;

        // Constructor
        public BlobDeadState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
            // Disable movement
            context.Blob.NavMeshAgent.speed = 0;
            context.Blob.NavMeshAgent.ResetPath();

            // Create mesh animation sequence
            Sequence sequence = DOTween.Sequence();
            sequence.Append(context.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), context.Blob.AnimationTime, 0));
            sequence.Append(context.transform.DOScale(Vector3.zero, context.Blob.AnimationTime));

            // Determine death fx duration
            float deathDuration = context.Blob.GetDeathAnimationDuration();
            _deathTimer = Time.time + deathDuration;    // little buffer, not sure if needed.

            // Play death FX
            sequence.Play();
            if (context.Blob.BlobType == BlobType.Bad || context.IsTransformed)
                context.Blob.DeathFX.gameObject.SetActive(true);

            // Update score
            if (ScoreManager.instanceExists)
                ScoreManager.Instance.AddPoints(context.IsTransformed ? -context.Blob.Points : context.Blob.Points, context.transform.position);
        }

        public override void UpdateState()
        {
            if (DeathTimerCheck() == false) return;

            // Death animations are complete, return to pool and cleanup.
            if (BlobManager.instanceExists)
                BlobManager.Instance.ReturnToPool(context.Blob);
        }

        public override void OnTriggerEnter(Collider other) { }

        public override void ExitState() { }
        #endregion

        #region Death
        private bool DeathTimerCheck()
        {
            if (Time.time < _deathTimer) return false; // Timer running
            // Timer done
            return true;
        }
        #endregion
    }
}