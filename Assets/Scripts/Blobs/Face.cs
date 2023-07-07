using ScriptableObjects;
using Sirenix.OdinInspector;
using StateMachine;
using UnityEngine;

namespace Blobs
{
    public class Face : MonoBehaviour
    {
        [InlineEditor()]
        [SerializeField] private FaceData _faceData;
        private Material _faceMaterial;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private BlobStateManager _stateManager;

        public Material FaceMaterial => _faceMaterial;

        #region Lifecycle

        private void Awake()
        {
            _faceMaterial ??= GetComponent<Blob>().BlobRenderer.materials[1];
            _stateManager ??= GetComponent<BlobStateManager>();
        }

        private void OnEnable()
        {
            _stateManager.OnStateChanged.AddListener(SwitchFace);
        }

        private void OnDisable()
        {
            _stateManager.OnStateChanged.RemoveListener(SwitchFace);
        }
        #endregion

        #region Face
        private void SwitchFace(BlobState state)
        {
            switch (state)
            {
                case BlobState.Idle:
                    SetFace(_faceData.IdleFace);
                    break;
                case BlobState.Patrol:
                    SetFace(_faceData.WalkFace);
                    break;
                case BlobState.Chase:
                    SetFace(_faceData.AttackFace);
                    break;
                case BlobState.Dead:
                    SetFace(_faceData.DamageFace);
                    break;
                case BlobState.Paused:
                default:
                    break;
            }
        }

        private void SetFace(Texture tex)
        {
            _faceMaterial.SetTexture(MainTex, tex);
        }
        #endregion
    }
}