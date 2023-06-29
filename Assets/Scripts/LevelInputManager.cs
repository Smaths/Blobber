using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelInputManager : MonoBehaviour
{
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private PlayerController _playerController;

    [Title("Pointer Settings")]
    [ReadOnly]
    [SerializeField]
    private Vector2 _currentPointerPosition;
    [Title("Misc Settings")]
    [DisplayAsString]
    [SerializeField]
    private bool _isOverUIObject;

    private GameInputActions _gameInputActions;

    // Public Properties
    public bool IsOverUIObject => _isOverUIObject;
    public Vector2 CurrentPointerPosition => _currentPointerPosition;

    #region Lifecycle
    private void Awake()
    {
        _gameInputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        _gameInputActions.Enable();

        _gameInputActions.UI.Cancel.performed += OnPausePerformed;
        _gameInputActions.Player.Move.performed += _playerController.OnMovePerformed;
        _gameInputActions.Player.Move.canceled += _playerController.OnMoveCanceled;
        _gameInputActions.Player.Boost.performed += _playerController.OnBoostInput;
    }

    private void OnDisable()
    {
        _gameInputActions.Disable();

        _gameInputActions.UI.Cancel.performed -= OnPausePerformed;
        _gameInputActions.Player.Move.performed -= _playerController.OnMovePerformed;
        _gameInputActions.Player.Move.canceled -= _playerController.OnMoveCanceled;
        _gameInputActions.Player.Boost.performed -= _playerController.OnBoostInput;
    }
    #endregion

    // Actions
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (ScoreManager.instance.GameIsOver) return;
        _gameTimer.TogglePause();
    }

    public void EnablePlayerInput(bool enable)
    {
        if (enable)
        {
            _gameInputActions.Player.Enable();
        }
        else
        {
            _gameInputActions.Player.Disable();
        }
    }
}