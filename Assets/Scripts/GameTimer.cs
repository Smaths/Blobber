using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    public static GameTimer instance;

    // Editor fields
    [Header("Time Settings")]
    [SuffixLabel("seconds"), MinValue(1)]
    [SerializeField] private float _timerDuration = 120f;
    [SerializeField, ReadOnly] private float _currentTime; // Current time remaining
    [SerializeField, ReadOnly] private bool _isPaused;

    // Events
    [Header("Events")]
    public UnityEvent OnCountdownCompleted;
    public UnityEvent<string> OnTimeChanged;
    public UnityEvent OnLevelPause;
    public UnityEvent OnLevelResume;

    public string CurrentTime
    {
        get
        {
            TimeSpan t = TimeSpan.FromSeconds(_currentTime);
            return $"{t.Minutes}m{t.Seconds}s";
        }
    }

    public bool IsPaused => _isPaused;

    #region Lifecycle
    private void OnValidate()
    {
        if (Application.isEditor)
        {
            TimeSpan t = TimeSpan.FromSeconds(_timerDuration);
            string timeString = $"{t.Minutes}m{t.Seconds}s";
            OnTimeChanged?.Invoke(timeString);
        }
    }

    private void Awake()
    {
        // Singleton setup
        if (instance != null) return;
        instance = this;
    }

    private void Start()
    {
        _currentTime = _timerDuration;
    }

    private void Update()
    {
        if (_isPaused) return;
        if (_currentTime <= 0f) return;  // Time ended

        _currentTime -= Time.deltaTime;

        TimeSpan t = TimeSpan.FromSeconds(_currentTime);
        string timeString = $"{t.Minutes}m{t.Seconds}s";
        print($"{gameObject.name} - OnTimeChanged:{timeString}");
        OnTimeChanged?.Invoke(timeString);

        if (_currentTime <= 0f) // Check if the countdown has reached zero
        {
            _currentTime = 0f;
            OnTimerFinished();
        }
    }
    #endregion

    public void Pause()
    {
        _isPaused = !_isPaused;

        print($"{gameObject.name} - Paused: {_isPaused}");

        if (_isPaused) OnLevelPause?.Invoke();
        else OnLevelResume?.Invoke();
    }

    private void OnTimerFinished()
    {
        Debug.Log("Countdown finished!");
        OnCountdownCompleted?.Invoke();
    }
}
