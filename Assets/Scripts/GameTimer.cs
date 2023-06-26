using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// Script execution order modified.

public class GameTimer : MonoBehaviour
{
    public static GameTimer instance;

    // Editor fields
    [Header("Time Settings")]
    [LabelText("Countdown Duration")]
    [SuffixLabel("second(s)"), MinValue(1)]
    [SerializeField] private float _countdownTimerDuration = 60f;
    [SuffixLabel("second(s)")]
    [SerializeField, ReadOnly] private float _currentTime; // Current time remaining
    [SerializeField, ReadOnly] private bool _isPaused;

    // Events
    [FoldoutGroup("Events", false)]
    public UnityEvent OnPreCountdownStarted;
    [FoldoutGroup("Events")]
    public UnityEvent OnCountdownStarted;
    [FoldoutGroup("Events")]
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
            TimeSpan current = TimeSpan.FromSeconds(_currentTime);
            string timeString = current.Minutes <= 0
                ? $"{current.Seconds}s"
                : $"{current.Minutes}m{current.Seconds}s";
            return timeString;
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
        _currentTime -= Time.deltaTime;

        TimeSpan current = TimeSpan.FromSeconds(_currentTime);

        string timeString = current.Minutes <= 0
            ? $"{current.Seconds}s"
            : $"{current.Minutes}m{current.Seconds}s";

        // print($"{gameObject.name} - OnTimeChanged:{timeString}");
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

        if (_isPaused) OnPause?.Invoke();
        else OnResume?.Invoke();
    }

    private void OnTimerFinished()
    {
        Debug.Log("Countdown finished!");
        OnCountdownCompleted?.Invoke();
    }
}
