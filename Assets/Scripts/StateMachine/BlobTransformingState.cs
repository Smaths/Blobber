using DG.Tweening;
using UnityEngine;

namespace StateMachine
{
    public class BlobTransformingState : BlobBaseState
    {
        private Sequence _transformSequence;

        // Constructor
        public BlobTransformingState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
            context.Blob.NavMeshAgent.isStopped = true;

            // Continue animation if paused
            if (_transformSequence != null && !_transformSequence.IsComplete() && !_transformSequence.IsPlaying())
            {
                _transformSequence.Play();
                return;
            }

            // Start new animation sequence.
            _transformSequence = context.IsTransformed
                ? TransformToGoodSequence(context.Blob.TransformationDuration)
                : TransformToBadSequence(context.Blob.TransformationDuration);
            _transformSequence.Play();
        }

        public override void UpdateState() { }

        public override void OnTriggerEnter(Collider other)
        {
            context.SwitchState(BlobState.Dead);
            _transformSequence = null;
        }

        public override void ExitState()
        {
            context.Blob.NavMeshAgent.isStopped = false;

            // Exiting because paused
            if (_transformSequence.IsPlaying())
                _transformSequence.Pause();
            else
                _transformSequence = null;

        }
        #endregion

        #region Transformation
        private Sequence TransformToBadSequence(float time)
        {
            float stageOneTime = time * 0.75f;
            float stageTwoTime = time * 0.25f;

            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0,context.BlobTransform.DOShakeRotation(stageOneTime, new Vector3(0, 45, 0), 5, 0, false));
            sequence.Insert(1, context.BlobTransform.DOPunchScale(new Vector3(0, -0.1f, 0), stageTwoTime, 3));
            sequence.Insert(1,context.Blob.BlobMaterial.DOColor(context.Blob.BadBlobColor, stageTwoTime));
            sequence.Insert(1,context.Blob.HeadAccessory.DOScale(0.8f, stageTwoTime).OnComplete(() =>
            {
                context.SetTransformed(!context.IsTransformed);
            }));
            sequence.AppendInterval(1.0f);   // Short delay after transformation is complete
            sequence.OnComplete(() => context.ReturnToPreviousState());
            return sequence;
        }

        // Modified from the transformation to bad sequence to benefit the player
        private Sequence TransformToGoodSequence(float time)
        {
            float stageOneTime = time * 0.75f;
            float stageTwoTime = time * 0.25f;

            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0,context.BlobTransform.DOShakeRotation(stageOneTime, new Vector3(0, 45, 0), 5, 0, false));
            sequence.Insert(1, context.BlobTransform.DOPunchScale(new Vector3(0, -0.1f, 0), stageTwoTime, 3));
            sequence.Insert(1,context.Blob.BlobMaterial.DOColor(context.Blob.GoodBlobColor, stageTwoTime));
            sequence.Insert(1,context.Blob.HeadAccessory.DOScale(0, stageTwoTime));
            sequence.InsertCallback(1, () => context.SetTransformed(!context.IsTransformed));
            sequence.AppendInterval(1.0f);   // Short delay after transformation is complete
            sequence.OnComplete(() => context.ReturnToPreviousState());
            return sequence;
        }
        #endregion
    }
}