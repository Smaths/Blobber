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
        [TitleGroup("Animation Times")]
        [HorizontalGroup("Animation Times/Times", LabelWidth = 60)] [LabelText("Fade In")] [MinValue(0)] [SuffixLabel("second(s)")]
        [SerializeField] private float _fadeInTime = 2.0f;
        [HorizontalGroup("Animation Times/Times", LabelWidth = 60)] [LabelText("Fade Out")] [MinValue(0)] [SuffixLabel("second(s)")]
        [SerializeField] private float _fadeOutTime = 3.0f;

        [Title("Scene Names")]
        [LabelText("Start")]
        [SerializeField] private string _startSceneName = "Start";
        [LabelText("Game")]
        [SerializeField] private string _gameSceneName = "Level_1";
        [LabelText("Tutorial")]
        [SerializeField] private string _tutorialSceneName = "Tutorial";

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        #region Events
        [FoldoutGroup("Public Events", false)] public UnityEvent OnFadeInStarted;
        [FoldoutGroup("Public Events")]public UnityEvent OnFadeInCompleted;
        [FoldoutGroup("Public Events")]public UnityEvent OnFadeOutStarted;
        [FoldoutGroup("Public Events")]public UnityEvent OnFadeOutCompleted;
        [FoldoutGroup("Public Events")]public UnityEvent OnFadeToStart;
        [FoldoutGroup("Public Events")]public UnityEvent OnFadeToLevel;
        #endregion

        #region Lifecycle
        protected override void Awake()
        {
            base.Awake();
            _canvas ??= GetComponentInChildren<Canvas>(true);
            _canvasGroup ??= _canvas.GetComponent<CanvasGroup>();
        }

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

        private void FadeTo(string sceneName, bool animated = true)
        {
            if (animated) FadeOut(sceneName);
            else SceneManager.LoadScene(sceneName);
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