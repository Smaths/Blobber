using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    // Editor fields
    [Tooltip("Current points of the player, game over if points go below 0.")]
    [SerializeField] private int _points;
    [SerializeField, ReadOnly] private bool _gameIsOver;

    // Events
    [FoldoutGroup("Events", false)]
    public UnityEvent<int, int> ScoreChanged;   // Amount changed, new total score
    [FormerlySerializedAs("OnGameOver_PlayerDead")]
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
        OnPlayerPointsDepleted?.Invoke();
    }
    #endregion
}