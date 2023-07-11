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

        [Title("Pointer")]
        [ReadOnly]
        [SerializeField] private Vector2 _currentPointerPosition;
        [DisplayAsString]
        [SerializeField] private bool _isOverUIObject;

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
        }

        private void OnDisable()
        {
            _gameInputActions.Disable();

            _gameInputActions.UI.Cancel.performed -= OnPausePerformed;
            _gameInputActions.Player.Move.performed -= OnMovePerformed;
            _gameInputActions.Player.Move.canceled -= OnMoveCanceled;
            _gameInputActions.Player.Boost.performed -= OnBoostPerformed;
        }
        #endregion

        // Actions
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            // Don't show pause as other UI screens are present when points depleted and/or time is up.
            if (ScoreManager.Instance.ArePointsDepleted) return;
            if (GameTimer.Instance.IsTimeUp) return;

            _gameTimer.TogglePause();
        }

        public void EnablePlayerInput(bool enable)
        {
            if (enable) _gameInputActions.Player.Enable();
            else _gameInputActions.Player.Disable();
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