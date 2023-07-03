using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace UI
{
    public class UI_GameOver : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TMP_Text _timeLabel;

        [Header("Fade Time")]
        [SerializeField] private float _fadeInTime = 0.3f;

        [FoldoutGroup("Events", false)]
        public UnityEvent OnResumeTapped;
        [FoldoutGroup("Events")]
        public UnityEvent OnRetryTapped;
        [FoldoutGroup("Events")]
        public UnityEvent OnLeaderBoardTapped;
        [FoldoutGroup("Events")]
        public UnityEvent OnQuitTapped;

        private void OnValidate()
        {
            _canvas ??= GetComponent<Canvas>();
            _canvasGroup ??= GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1f, _fadeInTime);

            SetTimeLabel();

            SubmitScoreToLeaderboard();
        }

        private void SetTimeLabel()
        {
            if (GameTimer.instance)
            {
                _timeLabel.text = GameTimer.instance.CurrentTime;
            }
        }

        #region Button Events
        public void Resume_Tapped()
        {
            OnResumeTapped?.Invoke();
            gameObject.SetActive(false);
        }
        public void Retry_Tapped()
        {
            _canvasGroup.interactable = false;
            OnRetryTapped?.Invoke();
            SceneFader.instance.FadeToCurrentScene();
        }

        public void Leaderboard_Tapped()
        {
            OnLeaderBoardTapped?.Invoke();
        }

        public void Quit_Tapped()
        {
            _canvasGroup.interactable = false;
            OnQuitTapped?.Invoke();
            SceneFader.instance.FadeToStart();
        }
        #endregion

        private void SubmitScoreToLeaderboard()
        {
            if (LootLockerTool.instanceExists && ScoreManager.instance.GameIsOver && ScoreManager.instance.Points > 0)
            {
                LootLockerTool.Instance.SubmitScore(ScoreManager.instance.Points);
            }
        }
    }
}