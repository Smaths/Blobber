using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    // Editor fields
    [Tooltip("Current points of the player, game over if points go below 0.")]
    [SerializeField] private int _points;
    [SerializeField, ReadOnly] private bool _gameIsOver;

    [Header("Bad Blob AI")]
    [SerializeField] private NavMeshAgent[] _badBlobAgents;

    // Events
    [FoldoutGroup("Events", false)]
    public UnityEvent<int, int> ScoreChanged;   // Amount changed, new total score
    [FoldoutGroup("Events")]
    public UnityEvent OnPointsAdd;
    [FoldoutGroup("Events")]
    public UnityEvent OnPointsSubtract;
    [FoldoutGroup("Events")]
    public UnityEvent OnPlayerPointsDepleted;

    #region Public Properties
    public int Points => _points;
    public bool GameIsOver => _gameIsOver;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Methods
    public void AddPoints(int value)
    {
        _points += value;

        if (value > 0)
        {
            OnPointsAdd?.Invoke();
        }
        else if (value < 0)
        {
            OnPointsSubtract?.Invoke();
        }
        ScoreChanged?.Invoke(value, _points);

        if (_points <= 0)
        {
            _points = 0;
            EndGame();
        }
    }

    public void EndGame()
    {
        _gameIsOver = true;

        DisableEnemies();

        OnPlayerPointsDepleted?.Invoke();
    }

    public void DisableEnemies()
    {
        if (_badBlobAgents.IsNullOrEmpty() == false)
        {
            foreach (var navMeshAgent in _badBlobAgents)
            {
                if (navMeshAgent == null) continue;
                navMeshAgent.speed = 0;
            }
        }
    }
    #endregion

    [Button]
    private void FindNavMeshAgents()
    {
        _badBlobAgents = FindObjectsOfType<NavMeshAgent>();
    }
}