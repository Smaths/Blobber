using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utility;

// Script execution order modified.
namespace Managers
{
    [RequireComponent(typeof(Timer))]
    public class GameTimer : Singleton<GameTimer>
    {
        // Editor fields
        [Header("Time Settings")]
        [LabelText("Countdown Duration")]
        [SuffixLabel("second(s)"), MinValue(1)]
        [SerializeField] private float _countdownDuration = 60f;
        [SuffixLabel("second(s)")]
        [SerializeField, DisplayAsString] private float _currentTime; // Current time remaining
        [SerializeField, ReadOnly] private bool _isPaused;

        [SuffixLabel("second(s)")] [MinValue(0)]
        [SerializeField] private float _preCountdownDuration = 3f;

        private Timer _timer;
        private bool _isStarted;
        private bool _isCompleted;

        #region Properties
        public bool IsPaused => _isPaused;
        public bool IsStarted => _isStarted;
        public bool IsCompleted => _isCompleted;
        public float CurrentTime => _currentTime;
        public string PrettyTime => NumberFormatter.FormatCleanTime(_timer.InvertedDuration);
        #endregion

        #region Events
        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnPause;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnResume;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent<string> OnPreCountdownTimeChanged;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnPreCountdownStarted;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnPreCountdownCompleted;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent<string> OnCountdownTimeChanged;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnCountdownStarted;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnCountdownCompleted;
        #endregion

        #region Lifecycle
        private void OnValidate()
        {
            if (Application.isPlaying == false)
                OnCountdownTimeChanged?.Invoke(NumberFormatter.FormatCleanTime(_currentTime));
        }

        protected override void Awake()
        {
            base.Awake();
            _timer ??= GetComponent<Timer>();
        }

        private void Start()
        {
            StartPreCountdownTimer();;
        }
        #endregion

        #region Public Methods
        public void TogglePause()
        {
            if (_isPaused)
            {
                _isPaused = false;
                _timer.Resume();
                OnResume?.Invoke();
            }
            else
            {
                _isPaused = true;
                _timer.Pause();
                OnPause?.Invoke();
            }
        }
        #endregion

        #region Timers
        // Pre-countdown countdown
        private void StartPreCountdownTimer()
        {
            _timer.secondChanged += OnPreCountdownDidUpdate;
            _timer.timerCompleted += OnPreCountdownDidComplete;
            _timer.StartTimer(_preCountdownDuration);

            OnPreCountdownStarted?.Invoke();
        }

        private void OnPreCountdownDidUpdate(int seconds)
        {
            OnPreCountdownTimeChanged?.Invoke(PrettyTime);
        }

        private void OnPreCountdownDidComplete()
        {
#if UNITY_EDITOR
            Debug.Log($"{gameObject.name} - Pre-countdown Finished. Game Start.");
#endif
            _isStarted = true;

            // Remove pre-countdown events
            _timer.secondChanged -= OnPreCountdownDidUpdate;
            _timer.timerCompleted -= OnPreCountdownDidComplete;

            OnPreCountdownCompleted?.Invoke();

            StartPrimaryCountdownTimer();
        }

        // Primary countdown
        private void StartPrimaryCountdownTimer()
        {
            _timer.secondChanged += OnCountdownDidUpdate;
            _timer.timerCompleted += OnCountdownDidComplete;
            _timer.StartTimer(_countdownDuration);

            OnCountdownStarted?.Invoke();
        }

        private void OnCountdownDidUpdate(int seconds)
        {
            OnCountdownTimeChanged?.Invoke(PrettyTime);
        }

        private void OnCountdownDidComplete()
        {
#if UNITY_EDITOR
            Debug.Log($"{gameObject.name} - Primary countdown Finished. Game is over.");
#endif
            _isCompleted = true;
            OnCountdownCompleted?.Invoke();
        }
        #endregion
    }
}
