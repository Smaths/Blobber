using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class BlobManager : Singleton<BlobManager>
{
    [Title("Blob Manager")]
    [SerializeField] private List<BlobAI> _blobs;
    [Header("Blob Prefabs")]
    [AssetsOnly, Required]
    [SerializeField] private GameObject _goodBlobPrefab;
    [AssetsOnly, Required]
    [SerializeField] private GameObject _badBlobPrefab;
    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private bool _showDebug;

    #region Lifecycle
    private void OnValidate()
    {
        FindBlobs();
        FindSpawnPoints();
    }

    protected override void Awake()
    {
        base.Awake();

        FindBlobs();
        FindSpawnPoints();

        Debug.Assert(_blobs != null || _blobs.Count > 0, $"{gameObject.name} doesn't have any blobs.");
    }
    #endregion

    #region Blobs
    public void DisableBlobs()
    {
        foreach (BlobAI blob in _blobs)
            blob.Disable();
    }

    public void EnableBlobs()
    {
        foreach (BlobAI blob in _blobs)
            blob.Enable();
    }

    [Button(ButtonSizes.Medium, Icon = SdfIconType.Search)]
    private void FindBlobs()
    {
        _blobs = FindObjectsOfType<BlobAI>().ToList();
    }
    #endregion

    #region Blob Event Handlers
    public void OnGoodBlobDestroyed(BlobAI blob)
    {
        if (_showDebug) print($"{gameObject.name} - Good Blob Destroyed {blob.name}");
        _blobs.Remove(blob);

        Instantiate(_goodBlobPrefab, RandomSpawnPointPosition(), quaternion.identity);
    }

    public void OnBadBlobDestroyed(BlobAI blob)
    {
        if (_showDebug)print($"{gameObject.name} - Bad Blob Destroyed {blob.name}");
        _blobs.Remove(blob);

        Instantiate(_badBlobPrefab, RandomSpawnPointPosition(), quaternion.identity);
    }
    #endregion

    #region Spawn Points
    [Button(ButtonSizes.Medium, Icon = SdfIconType.Search), PropertyOrder(4)]
    private void FindSpawnPoints()
    {
        _spawnPoints = transform.GetComponentsInChildren<Transform>();
    }

    private Vector3 RandomSpawnPointPosition()
    {
        if (_spawnPoints.IsNullOrEmpty()) return Vector3.zero;

        int randomIndex = Random.Range(0, _spawnPoints.Length - 1);
        return _spawnPoints[randomIndex].position;
    }
    #endregion
}
