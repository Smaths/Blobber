using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UI_GameOver : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TMP_Text _timeLabel;
        [SerializeField] private TMP_Text _scoreLabel;

        [Header("Fade Time")]
        [SerializeField] private float _fadeInTime = 0.3f;

        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnResumeTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnRetryTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnLeaderBoardTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnQuitTapped;

        private void OnValidate()
        {
            _canvas ??= GetComponent<Canvas>();
            _canvasGroup ??= GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1f, _fadeInTime);

            _scoreLabel.text = ScoreManager.Instance.PointsFormatted;
            _timeLabel.text = GameTimer.Instance.CurrentTimeFormatted;
        }

        #region Button Events
        public void Resume_Tapped()
        {
            OnResumeTapped?.Invoke();
            gameObject.SetActive(false);
            if (GameTimer.instanceExists)
                GameTimer.Instance.TogglePause();
        }

        public void Retry_Tapped()
        {
            _canvasGroup.interactable = false;
            OnRetryTapped?.Invoke();
            SceneFader.Instance.FadeToCurrentScene();
        }

        public void Leaderboard_Tapped()
        {
            OnLeaderBoardTapped?.Invoke();
        }

        public void Quit_Tapped()
        {
            _canvasGroup.interactable = false;
            OnQuitTapped?.Invoke();
            SceneFader.Instance.FadeToStart();
        }
        #endregion
    }
}