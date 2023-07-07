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
            
            string transform = context.IsTransformed ? "Good" : "Bad";
            Debug.Log($"Enter Transform - Will transform to {transform}");
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

        public override void ExitState()
        {
            Debug.Log("Exit Transform");
        }
        #endregion

        #region Transformation
        private Sequence TransformationSequence(float time)
        {
            float stageOneTime = time * 0.75f;
            float stageTwoTime = time * 0.25f;

            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0,context.Blob.BlobTransform.DOShakeRotation(stageOneTime, new Vector3(0, 45, 0), 5, 0, false));
            if (context.IsTransformed)
            {
                sequence.Insert(1, context.Blob.BlobTransform.DOPunchScale(new Vector3(0, -0.1f, 0), stageTwoTime, 3));
                sequence.Insert(1,context.Blob.BlobMaterial.DOColor(context.Blob.GoodBlobColor, stageTwoTime));
                sequence.Insert(1,context.Blob.Hat.DOScale(0, stageTwoTime));
            }
            else
            {
                sequence.Insert(1, context.Blob.BlobTransform.DOPunchScale(new Vector3(0, -0.1f, 0), stageTwoTime, 3));
                sequence.Insert(1,context.Blob.BlobMaterial.DOColor(context.Blob.BadBlobColor, stageTwoTime));
                sequence.Insert(1,context.Blob.Hat.DOScale(0.8f, stageTwoTime));
            }
            sequence.AppendInterval(1.0f);   // Short delay after transformation is complete
            return sequence;
        }

        private void OnTransformationComplete()
        {
            context.SetTransformed(!context.IsTransformed);
            context.ReturnToPreviousState();
            Debug.Log("Transform Complete");
        }
        #endregion
    }
}