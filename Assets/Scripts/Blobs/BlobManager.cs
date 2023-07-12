using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using ObjectPooling;
using Sirenix.OdinInspector;
using StateMachine;
using UnityEngine;
using Utility;

namespace Blobs
{
    [RequireComponent(typeof(PoolManager))]
    public class BlobManager : Utility.Singleton<BlobManager>
    {
        [TitleGroup("Blob Manager", "Spawn blobs from object pool.", TitleAlignments.Split)]
        [Tooltip("Additional number of objects spawned for object pooling. They are inactive on start.")]
        [MinValue(0)] [SuffixLabel("object(s)")]
        [SerializeField] private int _poolBufferAmount = 1; // Additional objects created in pool (inactive on start)
        [LabelText("Disable On Start")]
        [Tooltip("Disable blobs on level start, usually re-enabled when pre-countdown is complete.")]
        [SerializeField] private bool _shouldDisableOnStart = true;

        [TitleGroup("Good Blob Spawner", horizontalLine: false)]
        [SceneObjectsOnly] [LabelText("Container")]
        [Tooltip("Good blobs are placed here when spawned to keep the scene hierarchy clean.")]
        [SerializeField] private Transform _goodBlobContainer;

        [HorizontalGroup("Good Blob Spawner/Good")] [LabelText("Count")] [SuffixLabel("blob(s)")] [MinValue(0)]
        [SerializeField] private int _startCount_Good = 19;

        [HorizontalGroup("Good Blob Spawner/Good")] [AssetsOnly] [Required] [PreviewField(height:56), HideLabel]
        [SerializeField] private GameObject _goodBlobPrefab;

        [TitleGroup("Bad Blob Spawner", horizontalLine: false)]
        [SceneObjectsOnly] [LabelText("Container")]
        [Tooltip("Good blobs are placed here when spawned to keep the scene hierarchy clean.")]
        [SerializeField] private Transform _badBlobContainer;

        [HorizontalGroup("Bad Blob Spawner/Bad")] [LabelText("Count")] [SuffixLabel("blob(s)")] [MinValue(0)]
        [SerializeField] private int _startCount_Bad = 15;

        [HorizontalGroup("Bad Blob Spawner/Bad")] [AssetsOnly] [Required] [PreviewField(height:56), HideLabel]
        [SerializeField] private GameObject _badBlobPrefab;

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

        private void OnEnable()
        {
            // Attach events
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnCountdownStarted.AddListener(EnableBlobs);
                GameTimer.Instance.OnCountdownCompleted.AddListener(DisableBlobs);
                GameTimer.Instance.OnPause.AddListener(DisableBlobs);
                GameTimer.Instance.OnResume.AddListener(EnableBlobs);
            }

            if (ScoreManager.instanceExists)
            {
                ScoreManager.Instance.OnScoreIsZero.AddListener(DisableBlobs);
            }
        }

        private void OnDisable()
        {
            // Detach events
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnCountdownStarted.RemoveListener(EnableBlobs);
                GameTimer.Instance.OnCountdownCompleted.RemoveListener(DisableBlobs);
                GameTimer.Instance.OnPause.RemoveListener(DisableBlobs);
                GameTimer.Instance.OnResume.RemoveListener(EnableBlobs);
            }

            if (ScoreManager.instanceExists)
            {
                ScoreManager.Instance.OnScoreIsZero.RemoveListener(DisableBlobs);
            }
        }

        private void Start()
        {
            CreateBlobPools();

            if (_shouldDisableOnStart)
                DisableBlobs();
        }
        #endregion

        #region Blob Pools
        private void CreateBlobPools()
        {
            // Create pools
            PoolManager.Instance.CreatePool(_badBlobPrefab, _startCount_Bad + _poolBufferAmount, _badBlobContainer);
            PoolManager.Instance.CreatePool(_goodBlobPrefab, _startCount_Good + _poolBufferAmount, _goodBlobContainer);

            // Spawn objects
            for (int i = 0; i < _startCount_Good; i++)
                PoolManager.Instance.SpawnFromPool(_goodBlobPrefab, RandomSpawnPointPosition());
            for (int i = 0; i < _startCount_Bad; i++)
                PoolManager.Instance.SpawnFromPool(_badBlobPrefab, RandomSpawnPointPosition());
        }

        public void ReturnToPool(Blob blob)
        {
// #if UNITY_EDITOR
//             if (_showDebug) Debug.Log($"{gameObject.name} - {blob.name} ({blob.gameObject.GetInstanceID()}) returned to pool", transform);
// #endif
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

        #region Public Methods
        public void EnableBlobs()
        {
            List<GameObject> goodBlobs = PoolManager.Instance.GetPoolObjects(_goodBlobPrefab);
            List<GameObject> badBlobs = PoolManager.Instance.GetPoolObjects(_badBlobPrefab);

            foreach (GameObject blob in goodBlobs)
            {
                if (blob.activeInHierarchy == false) continue;

                var stateManager = blob.GetComponent<BlobStateManager>();
                if(stateManager != null)
                {
                    stateManager.ReturnToPreviousState();
                }
            }

            foreach (GameObject blob in badBlobs)
            {
                if (blob.activeInHierarchy == false) continue;

                var stateManager = blob.GetComponent<BlobStateManager>();
                if(stateManager != null)
                {
                    stateManager.ReturnToPreviousState();
                }
            }
        }

        public void DisableBlobs()
        {
            List<GameObject> goodBlobs = PoolManager.Instance.GetPoolObjects(_goodBlobPrefab);
            List<GameObject> badBlobs = PoolManager.Instance.GetPoolObjects(_badBlobPrefab);

            foreach (GameObject blob in goodBlobs)
            {
                if (blob.activeInHierarchy == false) continue;

                var stateManager = blob.GetComponent<BlobStateManager>();
                if(stateManager != null)
                {
                    stateManager.SwitchState(BlobState.Paused);
                }
            }

            foreach (GameObject blob in badBlobs)
            {
                if (blob.activeInHierarchy == false) continue;

                var stateManager = blob.GetComponent<BlobStateManager>();
                if(stateManager != null)
                {
                    stateManager.SwitchState(BlobState.Paused);
                }
            }
        }

        [ButtonGroup("Spawn")] [PropertyOrder(50)]
        public void SpawnGoodBlob()
        {
            PoolManager.Instance.SpawnFromPool(_goodBlobPrefab, RandomSpawnPointPosition());
        }

        [ButtonGroup("Spawn")] [PropertyOrder(50)]
        public void SpawnBadBlob()
        {
            PoolManager.Instance.SpawnFromPool(_badBlobPrefab, RandomSpawnPointPosition());
        }
        #endregion

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