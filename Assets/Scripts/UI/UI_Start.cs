using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace UI
{
    public class UI_Start : MonoBehaviour
    {
        [BoxGroup("Dependencies")]
        [SerializeField] private GameObject _setupCanvas;

        // Editor fields
        [SerializeField] private TMP_Text _playerNameLabel;
        [SerializeField] private bool _showDebug;
        [SerializeField] private GameObject _quitButton;

        // Private fields
        private CanvasGroup _canvasGroup;

        // Public events
        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnPlayerNameTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnPlayTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnTutorialTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnSettingsTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnLeaderboardTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnCreditsTapped;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent OnQuitTapped;

        #region Lifecycle
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _canvasGroup.alpha = 1;

            _playerNameLabel.text = string.IsNullOrEmpty(LootLockerTool.Instance.PlayerName)
                ? "Change Name"
                : LootLockerTool.Instance.PlayerName;
        }

        private void OnEnable()
        {
            // Disable quit button for WebGL
            _quitButton.gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);

            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnPlayerDataUpdated.AddListener(OnPlayerDataUpdated);
            }
        }

        private void OnDisable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnPlayerDataUpdated.RemoveListener(OnPlayerDataUpdated);
            }
        }
        #endregion

        #region Buttons Events
        public void PlayerName_Tapped()
        {
            OnPlayerNameTapped?.Invoke();
        }

        public void Start_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Start Tapped");

            if (string.IsNullOrEmpty(LootLockerTool.Instance.PlayerName))
            {
                // Request player name if missing
                _setupCanvas.gameObject.SetActive(true);
                _setupCanvas.GetComponent<UI_ChangePlayerName>().OnSubmit.AddListener(
                    delegate
                    {
                        SceneFader.Instance.FadeToGame();
                    }
                );
            }
            else
            {
                // Player name is set already, go to next scene.
                SceneFader.Instance.FadeToGame();;
            }

            OnPlayTapped?.Invoke();
        }

        public void Tutorial_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Tutorial Tapped");

            OnTutorialTapped?.Invoke();
        }

        public void Settings_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Settings Tapped");

            OnSettingsTapped?.Invoke();
        }

        public void Credits_Tapped()
        {
            if (_showDebug) print($"{gameObject.name} - Credits Tapped");

            OnCreditsTapped?.Invoke();
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

        private void OnPlayerDataUpdated()
        {
            _playerNameLabel.text = LootLockerTool.Instance.PlayerName;
        }
    }
}