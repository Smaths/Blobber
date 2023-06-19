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
    [SerializeField] private float _countdownTimerDuration = 120f;
    [SerializeField, ReadOnly] private float _currentTime; // Current time remaining
    [SerializeField, ReadOnly] private bool _isPaused;

    // Events
    [FoldoutGroup("Events", false)]
    public UnityEvent OnCountdownCompleted;
    [FoldoutGroup("Events")]
    public UnityEvent<string> OnTimeChanged;
    [FoldoutGroup("Events")]
    public UnityEvent OnPause;
    [FoldoutGroup("Events")]
    public UnityEvent OnResume;

    // Public properties
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
            TimeSpan t = TimeSpan.FromSeconds(_countdownTimerDuration);
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
        _currentTime = _countdownTimerDuration;
    }

    private void Update()
    {
        if (_isPaused) return;
        if (_currentTime <= 0f) return;  // Time ended

        // Decrement time (ignore application timescale).
        _currentTime -= Time.fixedDeltaTime;

        TimeSpan current = TimeSpan.FromSeconds(_currentTime);
        string timeString = $"{current.Minutes}m{current.Seconds}s";
        print($"{gameObject.name} - OnTimeChanged:{timeString}");
        OnTimeChanged?.Invoke(timeString);

        // Check if the countdown has reached zero
        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            OnTimerFinished();
        }
    }
    #endregion

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        print($"{gameObject.name} - Paused: {_isPaused}");

        if (_isPaused) OnPause?.Invoke();
        else OnResume?.Invoke();
    }

    private void OnTimerFinished()
    {
        Debug.Log("Countdown finished!");
        OnCountdownCompleted?.Invoke();
    }
}
