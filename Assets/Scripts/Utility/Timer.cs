using System;
using UnityEngine;

namespace Utility
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float _duration;
        [SerializeField] private float _currentTime;
        [SerializeField] private int _currentSecond;

        // Public properties
        public float Duration { get => _duration; private set => _duration = value; }
        public float InvertedDuration
        {
            get
            {
                return Duration - _currentTime;
            }
        }
        public bool IsTimerRunning { get; private set; }

        // Events
        public event Action timerCompleted;
        public event Action<int> secondChanged; // This event fires when the timer second changes

        #region Lifecycle
        private void Update()
        {
            if (!IsTimerRunning) return;

            _currentTime += Time.deltaTime;

            // Check if the number of seconds has changed. If so, call the SecondChanged event.
            if ((int) _currentTime != _currentSecond)
            {
                _currentSecond = (int) _currentTime;
                secondChanged?.Invoke(_currentSecond);
            }

            if (_currentTime >= Duration)
            {
                IsTimerRunning = false;
                timerCompleted?.Invoke();
            }
        }
        #endregion

        #region Public Methods
        public void StartTimer(float duration)
        {
            Duration = duration;
            _currentTime = 0f;
            IsTimerRunning = true;
        }

        public void StopTimer()
        {
            IsTimerRunning = false;
        }

        public void Pause()
        {
            if (IsTimerRunning) IsTimerRunning = false;
        }

        public void Resume()
        {
            if (!IsTimerRunning) IsTimerRunning = true;
        }
        #endregion
    }
}