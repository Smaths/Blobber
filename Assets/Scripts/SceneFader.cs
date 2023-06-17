using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;

    // Editor Fields
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _canvasGroup;

    [MinValue(0)] [SuffixLabel("s")]
    [Title("Settings")]
    [SerializeField] private float _fadeInTime = 2.0f;
    [MinValue(0)] [SuffixLabel("s")]
    [SerializeField] private float _fadeOutTime = 3.0f;

    [Title("Scene Names")]
    [SerializeField] private string _startSceneName = "Start";
    [SerializeField] private string _level1SceneName = "Level_1";

    [FoldoutGroup("Public Events", false)]
    public UnityEvent OnFadeInStarted;
    [FoldoutGroup("Public Events")]
    public UnityEvent OnFadeInCompleted;
    [FoldoutGroup("Public Events")]
    public UnityEvent OnFadeOutStarted;
    [FoldoutGroup("Public Events")]
    public UnityEvent OnFadeOutCompleted;
    [FoldoutGroup("Public Events")]
    public UnityEvent OnFadeToStart;
    [FoldoutGroup("Public Events")]
    public UnityEvent OnFadeToLevel;

    #region Lifecycle
    private void Awake()
    {
        // Singleton setup
        if (instance != null) return;
        instance = this;
    }

    private void Start()
    {
        _canvas.enabled = false;
    }
    #endregion

    #region Public Methods
    public void FadeToStart()
    {
        OnFadeToStart?.Invoke();
        instance.FadeTo(_startSceneName);
    }

    public void FadeToLevel1()
    {
        OnFadeToLevel?.Invoke();
        FadeTo(_level1SceneName);
    }

    public void FadeTo(string sceneName)
    {
        FadeOut(sceneName);
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