using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

namespace UI
{
    public class SceneFader : Singleton<SceneFader>
    {
        // Editor Fields
        [Header("UI Elements")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Fade Times")]
        [MinValue(0)] [SuffixLabel("s")]
        [Title("Settings")]
        [SerializeField] private float _fadeInTime = 2.0f;
        [MinValue(0)] [SuffixLabel("s")]
        [SerializeField] private float _fadeOutTime = 3.0f;

        [Header("Scene Names")]
        [SerializeField] private string _startSceneName = "Start";
        [SerializeField] private string _gameSceneName = "Level_1";
        [SerializeField] private string _tutorialSceneName = "Tutorial";

        [Header("Public Events")]
        public UnityEvent OnFadeInStarted;
        public UnityEvent OnFadeInCompleted;
        public UnityEvent OnFadeOutStarted;
        public UnityEvent OnFadeOutCompleted;
        public UnityEvent OnFadeToStart;
        public UnityEvent OnFadeToLevel;

        #region Lifecycle
        private void Start()
        {
            _canvas.enabled = false;
        }

        private void OnEnable()
        {
            FadeIn();
        }
        #endregion

        #region Public Methods
        public void FadeToStart(bool animated = true)
        {
            OnFadeToStart?.Invoke();
            FadeTo(_startSceneName);
        }

        public void FadeToGame()
        {
            OnFadeToLevel?.Invoke();
            FadeTo(_gameSceneName);
        }

        public void FadeToTutorial()
        {
            FadeTo(_tutorialSceneName, false);
        }

        public void FadeToCurrentScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            DOTween.Clear();
            FadeOut(scene.name);
        }

        public void FadeTo(string sceneName, bool animated = true)
        {
            if (animated)
                FadeOut(sceneName);
            else
                SceneManager.LoadScene(sceneName);
        }
        #endregion

        #region Private Methods
        private void FadeIn()
        {
            _canvas.enabled = true;

            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(0f, _fadeInTime)
                .OnStart(() => OnFadeInStarted?.Invoke())
                .OnComplete(() =>
                {
                    _canvas.enabled = false;
                    OnFadeInCompleted?.Invoke();
                });
        }

        private void FadeOut(string sceneName)
        {
            _canvas.enabled = true;

            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _fadeOutTime)
                .OnStart(() => OnFadeOutStarted?.Invoke())
                .OnComplete(() =>
                {
                    OnFadeOutCompleted?.Invoke();
                    SceneManager.LoadScene(sceneName);
                });
        }
        #endregion
    }
}