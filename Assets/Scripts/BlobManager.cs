using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Utility;
using Random = UnityEngine.Random;

public class BlobManager : Singleton<BlobManager>
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] _spawnPoints;

    [Header("Good Blobs")]
    [AssetsOnly, Required]
    [SerializeField] private GameObject _goodBlobPrefab;
    [SceneObjectsOnly]
    [SerializeField] private BlobAI[] _goodBlobs;

    [Header("Bad Blobs")]
    [AssetsOnly, Required]
    [SerializeField] private GameObject _badBlobPrefab;
    [SceneObjectsOnly]
    [SerializeField] private BlobAI[] _badBlobs;

    [SerializeField] private bool _showDebug;

    private List<BlobAI> _blobs;
    private List<NavMeshAgent> _blobAgents;

    #region Lifecycle
    private void OnValidate()
    {
        FindBlobs();
    }

    protected override void Awake()
    {
        base.Awake();

        FindBlobs();
    }
    #endregion

    [Button(ButtonSizes.Medium, Icon = SdfIconType.Search)]
    private void FindBlobs()
    {
        _blobs = FindObjectsOfType<BlobAI>().ToList();

        _goodBlobs = _blobs.Where(b => b.Type == BlobType.Good).ToArray();
        _badBlobs = _blobs.Where(b => b.Type == BlobType.Bad).ToArray();

        _blobAgents = new List<NavMeshAgent>();
        foreach (BlobAI blob in _blobs)
        {
            _blobAgents.Add(blob.Agent);
        }
    }

    public void DisableBlobs()
    {
        if (_blobs == null || _blobs.Count == 0) return;

        foreach (var blob in _blobs)
        {
            if (blob == null) continue;
            blob.Agent.speed = 0;   // Stop speed
        }
    }

    public void EnableBlobs()
    {
        if (_blobs == null || _blobs.Count == 0) return;

        foreach (var blob in _blobs)
        {
            if (blob == null) continue;
            blob.Agent.speed = blob.Speed;  // Reset speed
        }
    }

    public void OnGameOver()
    {
        DisableBlobs();
    }

    public void OnGoodBlobDestroyed(BlobAI blob)
    {
        if (_showDebug) print($"{gameObject.name} - Good Blob Destroyed {blob.name}");
        _blobs.Remove(blob);

        GameObject newBlob = Instantiate(_goodBlobPrefab, RandomSpawnpointPosition(), quaternion.identity);
    }

    public void OnBadBlobDestroyed(BlobAI blob)
    {
        if (_showDebug)print($"{gameObject.name} - Bad Blob Destroyed {blob.name}");
        _blobs.Remove(blob);

        GameObject newBlob = Instantiate(_badBlobPrefab, RandomSpawnpointPosition(), quaternion.identity);
    }

    private Vector3 RandomSpawnpointPosition()
    {
        if (_spawnPoints.IsNullOrEmpty()) return Vector3.zero;

        var randomIndex = Random.Range(0, _spawnPoints.Length - 1);
        return _spawnPoints[randomIndex].position;
    }
}
