using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BlobAI : MonoBehaviour
{
    [SerializeField] private int _pointValue = 5;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _playerBlob;
    [Tooltip("How far the blob can see.")]
    [SuffixLabel("seconds")]
    [SerializeField] private float _searchRate = 0.25f;
    [SerializeField] private float _searchRadius;
    [SerializeField] private LayerMask _searchMask;

    private bool _isDestroying;

    #region Lifecycle
    private void OnDrawGizmosSelected()
    {
        // Draw the search radius gizmo in the Unity Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }

    private void OnValidate()
    {
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;
    }

    private void Awake()
    {
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;
    }

    private void Start()
    {
        if (_navMeshAgent)
        {
            StartCoroutine(SearchForPlayerCoroutine());
        }
    }
    #endregion

    #region Collision
    private void OnCollisionEnter(Collision other)
    {
        print($"{gameObject.name} - {other.gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDestroying) return;  // guard statement

        print(_pointValue > 0
            ? $"{gameObject.name} Collision - Add {_pointValue}"
            : $"{gameObject.name} Collision - Subtract {_pointValue}");

        LevelManager.instance.AddPoints(_pointValue);

        // Animation
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f, vibrato: 0));
        sequence.Append(transform.DOScale(Vector3.zero, 0.2f));
        sequence.OnStart(() => _isDestroying = true);
        sequence.OnComplete(() => Destroy(gameObject, 0.1f));
        sequence.Play();
    }
    #endregion

    private IEnumerator SearchForPlayerCoroutine()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _searchRadius, _searchMask);

            foreach (Collider col in colliders)
            {
                // Hacky
                _navMeshAgent.SetDestination(col.transform.position);
            }
            yield return new WaitForSeconds(_searchRate);
        }
    }
}