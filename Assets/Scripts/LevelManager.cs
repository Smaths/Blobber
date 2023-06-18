using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private bool _gameIsOver;

    // Events
    [Header("Events")]
    public UnityEvent OnGameOver;

    #region Public Properties
    private int _points;

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

        if (_points <= 0)
        {
            _points = 0;
            EndGame();
        }
    }

    public void SubtractPoints(int value)
    {
        _points -= value;
    }

    public void EndGame()
    {
        _gameIsOver = true;
        OnGameOver?.Invoke();
    }
    #endregion
}