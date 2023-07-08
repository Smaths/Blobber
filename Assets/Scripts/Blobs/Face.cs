using System;
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

        #region Lifecycle

        private void Awake()
        {
            _faceMaterial ??= GetComponent<Blob>().BlobRenderer.materials[1];
            _stateManager ??= GetComponent<BlobStateManager>();
        }

        private void Start()
        {
            _stateManager.OnStateChanged.AddListener(SwitchFace);
        }

        private void OnDestroy()
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
                    break;
                case BlobState.Wander:
                    SetFace(_faceData.WalkFace);
                    break;
                case BlobState.Transforming:
                    SetFace(_faceData.DamageFace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void SetFace(Texture tex)
        {
            _faceMaterial.SetTexture(MainTex, tex);
        }
        #endregion
    }
}