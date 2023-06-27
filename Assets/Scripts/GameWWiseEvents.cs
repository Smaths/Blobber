using Sirenix.OdinInspector;
using UnityEngine;

public class GameWWiseEvents : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SceneObjectsOnly]
    [SerializeField] private PlayerController _playerBlob;
    [Title("WWise Game Events", "For hooking up player events and stuff to WWise", TitleAlignments.Split)]
    public AK.Wwise.Event OnScoreDecreaseEvent;
    public AK.Wwise.Event OnScoreIncreaseEvent;
    public AK.Wwise.Event OnBoostActivatedEvent;
    public AK.Wwise.Event OnGameStartEvent;
    public AK.Wwise.Event OnGamePauseEvent;
    public AK.Wwise.Event OnGameResumeEvent;
    public AK.Wwise.Event OnGameOverEvent;
    public AK.Wwise.Event OnDeadEvent;
    [Title("WWise UI Events", "For hooking up UI events and stuff to WWise", TitleAlignments.Split)]
    public AK.Wwise.Event OnButtonClickEvent;
    [SerializeField] private bool _showDebug;

    #region Lifecycle
    private void OnValidate()
    {
        _playerBlob ??= FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        if (GameTimer.instance)
        {
            GameTimer.instance.OnCountdownStarted.AddListener(OnCountdownStarted);
            GameTimer.instance.OnCountdownCompleted.AddListener(OnCountdownCompleted);
            GameTimer.instance.OnPause.AddListener(OnGamePause);
            GameTimer.instance.OnResume.AddListener(OnGameResume);
        }

        if (ScoreManager.instance)
        {
            ScoreManager.instance.OnScoreDecrease.AddListener(OnScoreDecrease);
            ScoreManager.instance.OnScoreIncrease.AddListener(OnScoreIncrease);
            ScoreManager.instance.OnPlayerPointsDepleted.AddListener(OnDead);
        }

        if (_playerBlob)
        {
            _playerBlob.OnBoostActivated.AddListener(OnPlayerBoostActivated);
        }
    }

    private void OnDisable()
    {
        if (GameTimer.instance)
        {
            GameTimer.instance.OnCountdownStarted.RemoveListener(OnCountdownStarted);
            GameTimer.instance.OnCountdownCompleted.RemoveListener(OnCountdownCompleted);
            GameTimer.instance.OnPause.RemoveListener(OnGamePause);
            GameTimer.instance.OnResume.RemoveListener(OnGameResume);
        }

        if (ScoreManager.instance)
        {
            ScoreManager.instance.OnScoreDecrease.RemoveListener(OnScoreDecrease);
            ScoreManager.instance.OnScoreIncrease.RemoveListener(OnScoreIncrease);
            ScoreManager.instance.OnPlayerPointsDepleted.RemoveListener(OnDead);
        }

        if (_playerBlob)
        {
            _playerBlob.OnBoostActivated.RemoveListener(OnPlayerBoostActivated);
        }
    }
    #endregion

    #region Methods
    private void OnScoreDecrease(int arg0)
    {
        if (_showDebug) print($"{gameObject.name} - On Score Decrease({arg0})");
        OnScoreDecreaseEvent.Post(gameObject);
    }

    private void OnScoreIncrease(int arg0)
    {
        if (_showDebug) print($"{gameObject.name} - On Score Increase({arg0})");
        OnScoreIncreaseEvent.Post(gameObject);
    }

    private void OnPlayerBoostActivated()
    {
        if (_showDebug) print($"{gameObject.name} - On Player Boosted)");
        OnBoostActivatedEvent.Post(gameObject);
    }

    private void OnCountdownStarted()
    {
        if (_showDebug) print($"{gameObject.name} - On Countdown Started)");
        OnGameStartEvent.Post(gameObject);
    }

    private void OnGameResume()
    {
        if (_showDebug) print($"{gameObject.name} - On Game Resume)");
        OnGameResumeEvent.Post(gameObject);
    }

    private void OnGamePause()
    {
        if (_showDebug) print($"{gameObject.name} - On Game Pause)");
        OnGamePauseEvent.Post(gameObject);
    }

    private void OnCountdownCompleted()
    {
        if (_showDebug) print($"{gameObject.name} - On Game Over)");
        OnGameOverEvent.Post(gameObject);
    }

    private void OnDead()
    {
        if (_showDebug) print($"{gameObject.name} - On Dead)");
        OnDeadEvent.Post(gameObject);
    }
    #endregion
}
