using DG.Tweening;
using Managers;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UI_CountdownTimer : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _countdownLabel;
        [SerializeField] private TMP_Text _preCountdownLabel;

        private void Start()
        {
            _preCountdownLabel.transform.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.AddListener(OnPreCountdownStarted);
                GameTimer.Instance.OnPreCountdownTimeChanged.AddListener(SetPreCountdownLabel);
                GameTimer.Instance.OnPreCountdownCompleted.AddListener(OnPreCountdownCompleted);
                GameTimer.Instance.OnCountdownTimeChanged.AddListener(SetTimeLabel);
            }
        }

        private void OnDisable()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.RemoveListener(OnPreCountdownStarted);
                GameTimer.Instance.OnPreCountdownTimeChanged.RemoveListener(SetPreCountdownLabel);
                GameTimer.Instance.OnPreCountdownCompleted.RemoveListener(OnPreCountdownCompleted);
                GameTimer.Instance.OnCountdownTimeChanged.RemoveListener(SetTimeLabel);
            }
        }

        // Pre-countdown timer
        private void OnPreCountdownStarted()
        {
            _preCountdownLabel.text = GameTimer.Instance.PrettyTime;
        }

        private void SetPreCountdownLabel(string timeString)
        {
            _preCountdownLabel.text = timeString;
        }

        private void OnPreCountdownCompleted()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_preCountdownLabel.DOText("Blob!", 0.2f));
            sequence.Append(_preCountdownLabel.DOScale(0, 0.25f).SetDelay(1.5f));
            sequence.Play();
        }

        // Primary timer
        private void SetTimeLabel(string timeString)
        {
            _countdownLabel.text = timeString;
        }
    }
}