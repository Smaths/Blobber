using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Blobs
{
    [RequireComponent(typeof(PoolManager))]
    public class BlobManager : Singleton<BlobManager>
    {
        [Title("Blob Manager")]
        [Title("Good Blobs", horizontalLine: false)]
        [AssetsOnly] [Required]
        [SerializeField] private GameObject _goodBlobPrefab;
        [SceneObjectsOnly]
        [SerializeField] private GameObject _goodBlobContainer;
        [LabelText("Count")] [SuffixLabel("blob(s)")]
        [SerializeField] private int _startCount_Good = 19;

        [Title("Bad Blobs", horizontalLine: false)]
        [AssetsOnly] [Required]
        [SerializeField] private GameObject _badBlobPrefab;
        [SceneObjectsOnly]
        [SerializeField] private GameObject _badBlobContainer;
        [LabelText("Count")] [SuffixLabel("blob(s)")]
        [SerializeField] private int _startCount_Bad = 15;

        [Title("Spawn Points")]
        [SerializeField] private ItemDistributor<Transform> _spawnPoints;

        [Space] [PropertyOrder(100)]
        [SerializeField] private bool _showDebug;

        private const int PoolBufferAmount = 5; // Additional objects created in pool (inactive on start)

        #region Lifecycle
        private void OnValidate()
        {
            FindSpawnPoints();
        }

        protected override void Awake()
        {
            base.Awake();

            FindSpawnPoints();
        }

        private void Start()
        {
            // Create pools
            PoolManager.Instance.CreatePool(_goodBlobPrefab, _startCount_Good + PoolBufferAmount, _goodBlobContainer.transform);
            PoolManager.Instance.CreatePool(_badBlobPrefab, _startCount_Bad + PoolBufferAmount, _badBlobContainer.transform);

            // Spawn objects
            for (int i = 0; i < _startCount_Good; i++)
                PoolManager.Instance.SpawnFromPool(_goodBlobPrefab, RandomSpawnPointPosition());
            for (int i = 0; i < _startCount_Bad; i++)
                PoolManager.Instance.SpawnFromPool(_badBlobPrefab, RandomSpawnPointPosition());
        }
        #endregion

        #region Blob Event Handlers
        public void OnBlobReturnToPool(Blob blob)
        {
#if UNITY_EDITOR
            if (_showDebug) print($"{gameObject.name} - ({blob.BlobType}) {blob.name} returned to pool");
#endif

            switch (blob.BlobType)
            {
                case BlobType.Good:
                    PoolManager.Instance.ReturnToPool(_goodBlobPrefab, blob.gameObject);
                    StartCoroutine(SpawnAfterDelayCoroutine(_goodBlobPrefab, 2f));
                    break;
                case BlobType.Bad:
                    PoolManager.Instance.ReturnToPool(_badBlobPrefab, blob.gameObject);
                    StartCoroutine(SpawnAfterDelayCoroutine(_badBlobPrefab, 2f));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator SpawnAfterDelayCoroutine(GameObject prefab, float time)
        {
            yield return new WaitForSeconds(time);
            PoolManager.Instance.SpawnFromPool(prefab, RandomSpawnPointPosition());
        }
        #endregion

        public void EnableBlobs()
        {
            Blob[] blobs = FindObjectsOfType<Blob>();
            foreach (var blob in blobs)
            {
                blob.Enable();
            }
        }

        public void DisableBlobs()
        {
            Blob[] blobs = FindObjectsOfType<Blob>();
            foreach (var blob in blobs)
            {
                blob.Disable();
            }
        }

        #region Spawn Points
        [Button(ButtonSizes.Medium, Icon = SdfIconType.Search)] [PropertyOrder(4)]
        private void FindSpawnPoints()
        {
            List<Transform> spawnPoints = transform.GetComponentsInChildren<Transform>().Where(sp => sp.gameObject.activeInHierarchy).ToList();
            _spawnPoints = new ItemDistributor<Transform>(spawnPoints);
        }

        private Vector3 RandomSpawnPointPosition()
        {
            return _spawnPoints.Count > 0
                ? _spawnPoints.GetNextItem().position
                : Vector3.zero;
        }
        #endregion
    }
}