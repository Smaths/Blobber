using System;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UI_CountdownTimer : MonoBehaviour
    {
        [SuffixLabel("seconds")]
        [SerializeField] private float _timerDuration = 120;
        [SerializeField] private TMP_Text _label;

        private float _currentTime; // Current time remaining

        private void Start()
        {
            _currentTime = _timerDuration;
        }

        private void Update()
        {
            if (_currentTime <= 0f) return;  // Time ended

            _currentTime -= Time.deltaTime;

            TimeSpan t = TimeSpan.FromSeconds(_currentTime);
            string timeString = $"{t.Minutes}m:{t.Seconds}s";
            _label.text = timeString;

            // Check if the countdown has reached zero
            if (_currentTime <= 0f)
            {
                // Countdown finished
                _currentTime = 0f;
                OnTimerFinished();
            }
        }

        private void OnTimerFinished()
        {
            // Implement your logic here for when the countdown finishes
            Debug.Log("Countdown finished!");
        }

    }
}