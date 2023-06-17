using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;

    // Editor Fields
    [MinValue(0)] [SuffixLabel("s")]
    [Title("Settings")]
    [SerializeField] private float _fadeInTime = 2.0f;
    [MinValue(0)] [SuffixLabel("s")]
    [SerializeField] private float _fadeOutTime = 3.0f;

    [Title("Scene Names")]
    [SerializeField] private string _startSceneName = "Start";
    [SerializeField] private string _otherSceneName = "Other";

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
    public UnityEvent OnFadeToOther;

    // Private fields
    private CanvasGroup _canvasGroup;

    #region Lifecycle
    private void Awake()
    {
        // Singleton setup
        if (instance != null) return;
        instance = this;

        _canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        _canvasGroup.alpha = 0;
    }
    #endregion

    #region Public Methods
    public void FadeToStart()
    {
        OnFadeToStart?.Invoke();
        instance.FadeTo(_startSceneName);
    }

    // Example for new scene
    public void FadeToOther()
    {
        OnFadeToOther?.Invoke();
        instance.FadeTo(_otherSceneName);
    }

    public void FadeTo(string sceneName)
    {
        FadeOut(sceneName);
    }
    #endregion

    #region Private Methods
    private void FadeIn()
    {
        Time.timeScale = 1f; // Reset the timeScale

        _canvasGroup.alpha = 1f;
        _canvasGroup.DOFade(0f, _fadeInTime)
            .OnStart(() => OnFadeInStarted?.Invoke())
            .OnComplete(() => OnFadeInCompleted?.Invoke());
    }

    private void FadeOut(string sceneName)
    {
        Time.timeScale = 1f; // Reset the timeScale

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