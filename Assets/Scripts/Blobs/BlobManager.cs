using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;

namespace Blobs
{
    [RequireComponent(typeof(PoolManager))]
    public class BlobManager : Singleton<BlobManager>
    {
        [Title("Blob Manager")]
        [Tooltip("Additional number of objects spawned for object pooling. They are inactive on start.")]
        [SerializeField] private int _poolBufferAmount = 1; // Additional objects created in pool (inactive on start)
        [Title("Good Blob Spawner", horizontalLine: false)]
        [LabelText("Count")] [SuffixLabel("blob(s)")]
        [SerializeField] private int _startCount_Good = 19;
        [AssetsOnly] [Required]
        [SerializeField] private GameObject _goodBlobPrefab;
        [SceneObjectsOnly]
        [SerializeField] private GameObject _goodBlobContainer;

        [Title("Bad Blob Spawner", horizontalLine: false)]
        [LabelText("Count")] [SuffixLabel("blob(s)")]
        [SerializeField] private int _startCount_Bad = 15;
        [AssetsOnly] [Required]
        [SerializeField] private GameObject _badBlobPrefab;
        [SceneObjectsOnly]
        [SerializeField] private GameObject _badBlobContainer;

        [Title("Spawn Points")]
        [SerializeField] private ItemDistributor<Transform> _spawnPoints;

        [Space] [PropertyOrder(100)]
        [SerializeField] private bool _showDebug;

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
            CreateBlobPools();
        }
        #endregion

        #region Blob Pools
        private void CreateBlobPools()
        {
            // Create pools
            PoolManager.Instance.CreatePool(_badBlobPrefab, _startCount_Bad + _poolBufferAmount, _badBlobContainer.transform);
            PoolManager.Instance.CreatePool(_goodBlobPrefab, _startCount_Good + _poolBufferAmount, _goodBlobContainer.transform);

            // Spawn objects
            for (int i = 0; i < _startCount_Good; i++)
                PoolManager.Instance.SpawnFromPool(_goodBlobPrefab, RandomSpawnPointPosition());
            for (int i = 0; i < _startCount_Bad; i++)
                PoolManager.Instance.SpawnFromPool(_badBlobPrefab, RandomSpawnPointPosition());
        }

        public void ReturnToPool(Blob blob)
        {
#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"{gameObject.name} - {blob.name} ({blob.gameObject.GetInstanceID()}) returned to pool", transform);
#endif
            GameObject prefab = blob.BlobType switch
            {
                BlobType.Good => _goodBlobPrefab,
                BlobType.Bad => _badBlobPrefab,
                _ => throw new ArgumentOutOfRangeException()
            };

            PoolManager.Instance.ReturnToPool(prefab, blob.gameObject);
            StartCoroutine(SpawnAfterDelayCoroutine(prefab, 2f));
        }

        private IEnumerator SpawnAfterDelayCoroutine(GameObject prefab, float time)
        {
            yield return new WaitForSeconds(time);

            GameObject foo = PoolManager.Instance.SpawnFromPool(prefab, RandomSpawnPointPosition());

#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"{gameObject.name} - {foo.name} ({foo.GetInstanceID()}) spawned from pool", transform);
#endif
        }
        #endregion

        public void EnableBlobs()
        {
            // TODO: Reimplement using blob state manager
            Blob[] blobs = FindObjectsOfType<Blob>();
            foreach (var blob in blobs)
            {
                // blob.Enable();
            }
        }

        public void DisableBlobs()
        {
            // TODO: Reimplement using blob state manager
            Blob[] blobs = FindObjectsOfType<Blob>();
            foreach (var blob in blobs)
            {
                // blob.Disable();
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
            return _spawnPoints.Count > 0 ? _spawnPoints.GetNextItem().position : Vector3.zero;
        }
        #endregion
    }
}