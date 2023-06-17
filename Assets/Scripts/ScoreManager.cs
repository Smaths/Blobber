using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public UnityEvent OnGameOver;

    // Public properties
    private int _points;

    public int Points
    {
        get
        {
            return _points;
        }
        private set
        {
            // Prevent negative points
            if (_points <= 0)
            {
                print($"{gameObject.name} - Game Over");
                OnGameOver?.Invoke();
                _points = 0;
                return;
            }

            _points = value;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this); // Persist the object on scene unload
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Points = 0;
    }

    public void AddPoints(int value)
    {
        Points += value;
    }

    public void SubtractPoints(int value)
    {
        Points -= value;
    }
}