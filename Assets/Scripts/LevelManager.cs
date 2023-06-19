using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    // Editor fields
    [SerializeField] private int _points;
    [SerializeField] private bool _gameIsOver;

    // Events
    [Header("Events")]
    public UnityEvent<int> ScoreChanged;
    public UnityEvent OnGameOver;

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

        ScoreChanged?.Invoke(value);

        if (_points <= 0)
        {
            _points = 0;
            EndGame();
        }
    }

    public void SubtractPoints(int value)
    {
        _points -= value;

        ScoreChanged?.Invoke(value);

        if (_points <= 0)
        {
            _points = 0;
            EndGame();
        }
    }

    public void EndGame()
    {
        _gameIsOver = true;
        OnGameOver?.Invoke();
    }
    #endregion
}