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

        private void OnEnable()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.AddListener(OnPreCountdownStarted);
                GameTimer.Instance.OnPreCountdownTimeChanged.AddListener(OnPreCountdownTimeChanged);
                GameTimer.Instance.OnPreCountdownCompleted.AddListener(OnPreCountdownCompleted);
                GameTimer.Instance.OnCountdownTimeChanged.AddListener(SetTimeLabel);
            }
        }

        private void Start()
        {
            _preCountdownLabel.transform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.RemoveListener(OnPreCountdownStarted);
                GameTimer.Instance.OnPreCountdownTimeChanged.RemoveListener(OnPreCountdownTimeChanged);
                GameTimer.Instance.OnPreCountdownCompleted.RemoveListener(OnPreCountdownCompleted);
                GameTimer.Instance.OnCountdownTimeChanged.RemoveListener(SetTimeLabel);
            }
        }

        // Pre-countdown timer
        private void OnPreCountdownStarted()
        {
            _preCountdownLabel.text = GameTimer.Instance.PrettyTime;
        }

        private void OnPreCountdownTimeChanged(string timeString)
        {
            _preCountdownLabel.text = timeString;

            // Animation
            _preCountdownLabel.transform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 2);
            _preCountdownLabel.transform.DOScale(transform.localScale.x * 1.5f , 0.2f);
            int randomRotation = _preCountdownLabel.transform.rotation.z < 0 ? Random.Range(2, 10) : Random.Range(-10, -2);
            Vector3 position = _preCountdownLabel.transform.position;
            _preCountdownLabel.transform.DORotate(new Vector3(0, 0, randomRotation), 0.4f);
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