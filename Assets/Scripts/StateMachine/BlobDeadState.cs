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
            context.Blob.NavMeshAgent.ResetPath();

            AddPoints();

            ActivateDeathFX();
        }

        public override void UpdateState() { }

        public override void OnTriggerEnter(Collider other) { }

        public override void ExitState() { }
        #endregion

        #region Private Methods
        private void AddPoints()
        {
            if (ScoreManager.instanceExists)
                ScoreManager.Instance.AddPoints(context.IsTransformed ? -context.Blob.Points : context.Blob.Points, context.transform.position);
        }

        private void ActivateDeathFX()
        {
            // Explosion FX
            if (context.Blob.BlobType == BlobType.Bad || context.IsTransformed)
                context.Blob.DeathFX.gameObject.SetActive(true);

            // Scale
            Sequence sequence = DOTween.Sequence();
            sequence.Append(context.BlobTransform.DOScale(Vector3.zero, context.Blob.AnimationTime));
            sequence.OnComplete(() =>
            {
                if (BlobManager.instanceExists) BlobManager.Instance.ReturnToPool(context.Blob);
                context.BlobTransform.localScale = Vector3.one;
            });
            sequence.Play();
        }
        #endregion
    }
}