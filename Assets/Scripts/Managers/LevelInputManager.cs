using Blobs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Managers
{
    public class LevelInputManager : Singleton<LevelInputManager>
    {
        [SerializeField] private GameTimer _gameTimer;
        [SerializeField] private PlayerController _playerController;

        [Header("Pointer")]
        [DisplayAsString]
        [SerializeField] private bool _isOverUIObject;
        [ReadOnly]
        [SerializeField] private Vector2 _currentPointerPosition;

        private GameInputActions _gameInputActions;

        // Public Properties
        public bool IsOverUIObject => _isOverUIObject;
        public Vector2 CurrentPointerPosition => _currentPointerPosition;

        #region Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _gameInputActions = new GameInputActions();
        }

        private void OnEnable()
        {
            _gameInputActions.Enable();

            _gameInputActions.UI.Cancel.performed += OnPausePerformed;
            _gameInputActions.Player.Move.performed += OnMovePerformed;
            _gameInputActions.Player.Move.canceled += OnMoveCanceled;
            _gameInputActions.Player.Boost.performed += OnBoostPerformed;

            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.AddListener(DisablePlayerInput);
                GameTimer.Instance.OnCountdownStarted.AddListener(EnablePlayerInput);
            }
        }

        private void OnDisable()
        {
            _gameInputActions.Disable();

            _gameInputActions.UI.Cancel.performed -= OnPausePerformed;
            _gameInputActions.Player.Move.performed -= OnMovePerformed;
            _gameInputActions.Player.Move.canceled -= OnMoveCanceled;
            _gameInputActions.Player.Boost.performed -= OnBoostPerformed;

            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.RemoveListener(DisablePlayerInput);
                GameTimer.Instance.OnCountdownStarted.RemoveListener(EnablePlayerInput);
            }
        }
        #endregion

        #region Public Methods
        public void EnablePlayerInput(bool enable)
        {
            if (enable) _gameInputActions.Player.Enable();
            else _gameInputActions.Player.Disable();
        }
        #endregion

        // Actions
        private void EnablePlayerInput()
        {
            _gameInputActions.Player.Enable();
        }

        private void DisablePlayerInput()
        {
            _gameInputActions.Player.Disable();
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (GameTimer.Instance.IsStarted == false) return;
            if (GameManager.Instance.IsGameOver) return;

            _gameTimer.TogglePause();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (_playerController) _playerController.OnMovePerformed(context);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (_playerController) _playerController.OnMoveCanceled(context);
        }

        private void OnBoostPerformed(InputAction.CallbackContext context)
        {
            if (_playerController) _playerController.OnBoostInput(context);
        }
    }
}