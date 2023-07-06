using Blobs;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace StateMachine
{
    public class BlobDeadState : BlobBaseState
    {
        private float _deathTimer;

        // Constructor
        public BlobDeadState(BlobStateManager context) : base(context) { }

        // Required State Methods
        public override void EnterState()
        {
            ScoreManager.instance.AddPoints(context.Blob);

            // Create mesh animation sequence
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                context.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), context.Blob.AnimationTime, 0));
            sequence.Append(context.transform.DOScale(Vector3.zero, context.Blob.AnimationTime));

            // Determine death fx duration
            float deathDuration = context.Blob.GetDeathAnimationDuration();
            _deathTimer = Time.time + deathDuration;

            // Play death FX
            sequence.Play();
            if (context.Blob.BlobType == BlobType.Bad || context.Blob.IsTransformed)
                context.Blob.DeathFX.gameObject.SetActive(true);
        }

        public override void UpdateState()
        {
            if (DeathTimerCheck() == false) return;

            if (!BlobManager.instanceExists) BlobManager.Instance.ReturnToPool(context.Blob);
        }

        private bool DeathTimerCheck()
        {
            if (Time.time < _deathTimer) return false; // Timer running
            // Timer done
            return true;
        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        // Do nothing... dead.
        public override void OnCollisionEnter(Collision other) { }
    }
}