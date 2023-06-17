using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI_Start : MonoBehaviour
    {
        // Editor fields
        [SerializeField] private float _fadeTime;
        [SerializeField] private bool _showDebug;

        // Private fields
        private CanvasGroup _canvasGroup;

        // Public events
        public UnityEvent OnStartTapped;
        public UnityEvent OnLeaderboardTapped;
        public UnityEvent OnQuitTapped;

        #region Lifecycle
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _canvasGroup.alpha = 1;
        }
        #endregion

        public void Close()
        {
            //_canvasGroup.DOFade(0, 1.0f);
        }

        #region Buttons Events
        public void Start_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Start Tapped");

            Close();
            SceneFader.instance.FadeToLevel1();

            OnStartTapped?.Invoke();
        }

        public void Leaderboard_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Leaderboard Tapped");
            OnLeaderboardTapped?.Invoke();
        }

        public void Quit_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Quit Tapped");
            OnQuitTapped?.Invoke();

            Application.Quit();
        }
        #endregion
    }
}