using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum BlobAIState { Idle, Patrol, Attack, Dead }

public class BlobAI : MonoBehaviour
{
    [FormerlySerializedAs("_navMeshAgent")]
    [BoxGroup("Dependencies")]
    [SerializeField] private NavMeshAgent _agent;
    [Title("Blob Setting")]
    [SerializeField] private int _pointValue = 5;
    [SerializeField] private GameObject _playerBlob;
    [Tooltip("How far the blob can see.")]
    [SuffixLabel("second(s)")]
    [SerializeField] private float _searchRate = 0.25f;
    [SuffixLabel("meter(s)")]
    [SerializeField] private float _searchRadius = 4f;
    [SerializeField] private LayerMask _searchMask;
    [SerializeField] private float _patrolRange = 4f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Face")]
    [SerializeField] private GameObject _blobMesh;
    [FormerlySerializedAs("faces")]
    [SerializeField] private Face _faces;
    [SerializeField] private BlobAIState _currentState;
    [SerializeField] private Material _faceMaterial;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private bool _isDestroying;
    private Vector3 _targetDestination;
    private bool _hasTarget;
    private bool _hasPatrolPoint;

    public UnityEvent<BlobAIState> OnBlobStateChange;
    public UnityEvent OnGainedTarget;
    public UnityEvent OnLostTarget;
    public UnityEvent OnIdle;
    public UnityEvent OnPatrol;
    public UnityEvent OnAttack;
    public UnityEvent OnDeath;
    private bool _hadTarget;

    public BlobAIState CurrentState
    {
        get => _currentState;
        private set
        {
            switch (value)
            {
                case BlobAIState.Idle:
                    OnIdle?.Invoke();
                    break;
                case BlobAIState.Patrol:
                    OnPatrol?.Invoke();
                    break;
                case BlobAIState.Attack:
                    OnAttack?.Invoke();
                    break;
                case BlobAIState.Dead:
                    OnDeath?.Invoke();
                    break;
            }

            if (_faceMaterial)
            {
                UpdateFade(value);
            }

            OnBlobStateChange?.Invoke(value);

            _currentState = value;
        }
    }

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

        if (_blobMesh)
        {
            if (_faces != null)
            {
                if (_faceMaterial)
                    CurrentState = _currentState;
            }
        }
    }

    private void Awake()
    {
        _playerBlob ??= FindObjectOfType<PlayerController>().gameObject;

        if (_blobMesh)
        {
            if (_faces != null)
            {
                _faceMaterial = _blobMesh.GetComponent<Renderer>().materials[1];

                if (_faceMaterial)
                    CurrentState = _currentState;
            }
        }
    }

    private void Start()
    {
        if (_agent)
        {
            StartCoroutine(SearchForPlayerCoroutine());
        }

        CurrentState = BlobAIState.Idle;
    }
    #endregion

    #region Collider Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (_isDestroying) return;  // guard statement

        print(_pointValue > 0
            ? $"Player hit {gameObject.name} (+ {_pointValue})"
            : $"Player hit {gameObject.name} (- {_pointValue})");

        LevelManager.instance.AddPoints(_pointValue);

        CurrentState = BlobAIState.Dead;

        PlayDeathAnimation();
    }

    private void PlayDeathAnimation()
    {
        // Animation
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f, vibrato: 0));
        sequence.Append(transform.DOScale(Vector3.zero, 0.2f));
        sequence.OnStart(() => _isDestroying = true);
        sequence.OnComplete(() => Destroy(gameObject, 0.1f));
        sequence.Play();
    }
    #endregion

    #region Search for Player
    private IEnumerator SearchForPlayerCoroutine()
    {
        while (!_isDestroying )
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _searchRadius, _searchMask);

            if (colliders.IsNullOrEmpty())
            {
                // No Target
                _hasTarget = false;
                _targetDestination = transform.position;
                CurrentState = BlobAIState.Patrol;
                if (_hadTarget)
                {
                    _hadTarget = false;
                    OnLostTarget?.Invoke();
                }
            }
            else
            {
                // Set Target
                _hasTarget = true;
                _hadTarget = true;
                _targetDestination = colliders[0].transform.position;
                CurrentState = BlobAIState.Attack;
                OnGainedTarget?.Invoke();
            }

            _agent.SetDestination(_targetDestination);

            yield return new WaitForSeconds(_searchRate);
        }
    }
    #endregion

    #region Patrol
    private void SearchForPatrolPoint()
    {
        float randomX = Random.Range(-_patrolRange, _patrolRange);
        float randomZ = Random.Range(-_patrolRange, _patrolRange);

        Vector3 cachedPosition = transform.position;
        _targetDestination = new Vector3(cachedPosition.x + randomX, cachedPosition.y, cachedPosition.z + randomZ);

        if (Physics.Raycast(_targetDestination, -transform.up, 2f, _groundMask))
        {
            _hasPatrolPoint = true;
        }
    }

    private void Patrol()
    {
        if (!_hasPatrolPoint)
            SearchForPatrolPoint();

        if (_hasPatrolPoint)
            _agent.SetDestination(_targetDestination);

        // Patrol point reached
        Vector3 distanceToPatrolPoint = transform.position - _targetDestination;
        if (distanceToPatrolPoint.magnitude < 1f)
        {
            _hasPatrolPoint = false;
        }
    }
    #endregion

    #region Face
    private void UpdateFade(BlobAIState state)
    {
        switch (state)
        {
            case BlobAIState.Idle:
                SetFace(_faces.Idleface);
                break;
            case BlobAIState.Patrol:
                SetFace(_faces.WalkFace);
                break;
            case BlobAIState.Attack:
                SetFace(_faces.attackFace);
                break;
            case BlobAIState.Dead:
                SetFace(_faces.damageFace);
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