using System;
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
        [Title("Good Blobs", horizontalLine: false)]
        [AssetsOnly] [Required]
        [SerializeField] private GameObject _goodBlobPrefab;
        [SerializeField] private GameObject _goodBlobContainer;
        [HorizontalGroup("Good"), LabelText("Start Count")]
        [SerializeField] private int _defaultCapacity_Good = 19;
        [HorizontalGroup("Good"), LabelText("Max")]
        [SerializeField] private int _maxCapacity_Good = 20;

        [Title("Bad Blobs", horizontalLine: false)]
        [AssetsOnly] [Required]
        [SerializeField] private GameObject _badBlobPrefab;
        [SerializeField] private GameObject _badBlobContainer;
        [HorizontalGroup("Bad"), LabelText("Start Count")]
        [SerializeField] private int _defaultCapacity_Bad = 15;
        [HorizontalGroup("Bad"), LabelText("Max")]
        [SerializeField] private int _maxCapacity_Bad = 16;

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
            // Create pools
            PoolManager.Instance.CreatePool(_goodBlobPrefab, _maxCapacity_Good, _goodBlobContainer.transform);
            PoolManager.Instance.CreatePool(_badBlobPrefab, _maxCapacity_Bad, _badBlobContainer.transform);

            // Spawn objects
            for (int i = 0; i < _defaultCapacity_Good; i++)
                PoolManager.Instance.SpawnFromPool(_goodBlobPrefab, RandomSpawnPointPosition());
            for (int i = 0; i < _defaultCapacity_Bad; i++)
                PoolManager.Instance.SpawnFromPool(_badBlobPrefab, RandomSpawnPointPosition());
        }
        #endregion

        #region Blob Event Handlers
        public void OnBlobReturnToPool(BlobAI blob)
        {
#if UNITY_EDITOR
            if (_showDebug) print($"{gameObject.name} - ({blob.Type}) {blob.name} returned to pool");
#endif

            switch (blob.Type)
            {
                case BlobType.Good:
                    PoolManager.Instance.ReturnToPool(_goodBlobPrefab, blob.gameObject);
                    PoolManager.Instance.SpawnFromPool(_goodBlobPrefab, RandomSpawnPointPosition());
                    break;
                case BlobType.Bad:
                    PoolManager.Instance.ReturnToPool(_badBlobPrefab, blob.gameObject);
                    PoolManager.Instance.SpawnFromPool(_badBlobPrefab, RandomSpawnPointPosition());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        public void EnableBlobs()
        {
            BlobAI[] blobs = FindObjectsOfType<BlobAI>();
            foreach (var blob in blobs)
            {
                blob.Enable();
            }
        }

        public void DisableBlobs()
        {
            BlobAI[] blobs = FindObjectsOfType<BlobAI>();
            foreach (var blob in blobs)
            {
                blob.Disable();
            }
        }

        #region Spawn Points
        [Button(ButtonSizes.Medium, Icon = SdfIconType.Search)] [PropertyOrder(4)]
        private void FindSpawnPoints()
        {
            List<Transform> spawnPoints = transform.GetComponentsInChildren<Transform>().ToList();
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