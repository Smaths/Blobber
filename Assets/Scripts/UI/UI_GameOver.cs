using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_GameOver : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TMP_Text _timeLabel;
        [SerializeField] private TMP_Text _scoreLabel;
        [SerializeField] private TMP_Text _newHighScoreLabel;

        [SerializeField] private Button _firstButton;

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
            _timeLabel.text = GameTimer.Instance.PrettyTime;

            if (_newHighScoreLabel)
            {
                if (LootLockerTool.Instance.PreviousHighScore < ScoreManager.Instance.Points)
                {
                    _newHighScoreLabel.gameObject.SetActive(true);
                    _newHighScoreLabel.transform.DOPunchScale(Vector3.one * 0.25f, 4, 3);
                }
                else
                {
                    _newHighScoreLabel.gameObject.SetActive(false);
                }
            }

            if (_firstButton)
                EventSystem.current.SetSelectedGameObject(_firstButton.gameObject);
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