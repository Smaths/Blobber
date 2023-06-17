using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    // Public properties
    private int _points;
    public int Points => _points;

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
        _points = 0;
    }

    public void AddPoints(int value)
    {
        _points += value;
    }

    public void SubtractPoints(int value)
    {
        _points -= value;
    }
}