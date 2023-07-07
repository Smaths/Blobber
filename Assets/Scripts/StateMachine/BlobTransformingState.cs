using DG.Tweening;
using UnityEngine;

namespace StateMachine
{
    public class BlobTransformingState : BlobBaseState
    {
        // Constructor
        public BlobTransformingState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
            context.Blob.NavMeshAgent.speed = 0;
            context.Blob.NavMeshAgent.ResetPath();

            // Animate transformation
            Sequence sequence = TransformationSequence(context.Blob.TransformationTime);
            sequence.OnComplete(OnTransformationComplete);
            sequence.Play();
        }

        public override void UpdateState() { }

        public override void OnTriggerEnter(Collider other)
        {
            context.SwitchState(BlobState.Dead);
        }

        public override void ExitState() { }
        #endregion

        #region Transformation
        private Sequence TransformationSequence(float time)
        {
            float stageOneTime = time * 0.60f;
            float stageTwoTime = time * 0.2f;
            float stageThreeTime = time * 0.2f;

            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0,context.Blob.BlobMesh.transform.DOShakeRotation(stageOneTime, new Vector3(0, 45, 0), 5, 0, false));
            if (context.IsTransformed)
            {
                sequence.Insert(1, context.Blob.BlobMesh.transform.DOPunchScale(new Vector3(0, 0, -0.1f), stageTwoTime, 3));
                sequence.Insert(2,context.Blob.BlobMaterial.DOColor(context.Blob.GoodBlobColor, stageThreeTime));
                sequence.Insert(2,context.Blob.Hat.transform.DOScale(0, stageThreeTime));
            }
            else
            {
                sequence.Insert(1, context.Blob.BlobMesh.transform.DOPunchScale(new Vector3(0, 0, -0.1f), stageTwoTime, 3));
                sequence.Insert(2,context.Blob.BlobMaterial.DOColor(context.Blob.BadBlobColor, stageThreeTime));
                sequence.Insert(2,context.Blob.Hat.transform.DOScale(0.8f, stageThreeTime));
            }

            return sequence;
        }

        private void OnTransformationComplete()
        {
            context.ReturnToPreviousState();
            context.SetTransformed(!context.IsTransformed);
        }
        #endregion
    }
}