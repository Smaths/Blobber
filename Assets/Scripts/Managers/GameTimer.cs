using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utility;

// Script execution order modified.
namespace Managers
{
    public class GameTimer : Singleton<GameTimer>
    {
        // Editor fields
        [Header("Time Settings")]
        [LabelText("Countdown Duration")]
        [SuffixLabel("second(s)"), MinValue(1)]
        [SerializeField] private float _countdownTimerDuration = 60f;
        [SuffixLabel("second(s)")]
        [SerializeField, DisplayAsString] private float _currentTime; // Current time remaining
        [SerializeField, ReadOnly] private bool _isPaused;

        #region Properties
        public string CurrentTimeFormatted
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
        public bool IsTimeUp => _currentTime <= 0;
        #endregion

        #region Events
        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent<string> OnTimeChanged;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnPause;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnResume;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnPreCountdownStarted;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnCountdownStarted;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnCountdownCompleted;
        #endregion

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

        private void Start()
        {
            _currentTime = _countdownTimerDuration;
            OnCountdownStarted?.Invoke();
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

        #region Public Methods
        public void Stop()
        {
            _isPaused = true;
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;

            if (_isPaused) OnPause?.Invoke();
            else OnResume?.Invoke();
        }
        #endregion

        private void OnTimerFinished()
        {
#if UNITY_EDITOR
            Debug.Log($"{gameObject.name} - Countdown Finished. Game is over.");
#endif

            GameManager.Instance.IsGameOver = true;

            OnCountdownCompleted?.Invoke();
        }
    }
}
