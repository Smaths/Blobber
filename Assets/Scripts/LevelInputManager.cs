using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelInputManager : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private PlayerController _playerController;

    [Title("Pointer Settings")]
    [ReadOnly]
    [SerializeField]
    private Vector2 _currentPointerPosition;
    [Title("Click Settings")]
    [SerializeField] private bool _areControlsEnabled;
    [SerializeField]
    private LayerMask _clickLayerMask;
    [Title("Misc Settings")]
    [DisplayAsString]
    [SerializeField]
    private bool _isOverUIObject;
    [SerializeField]
    private bool _showDebugMessages;

    private GameInputActions _gameInputActions;
    private UnityEngine.Camera _camera;

    // Public Properties
    public bool IsOverUIObject => _isOverUIObject;
    public Vector2 CurrentPointerPosition => _currentPointerPosition;
    public bool AreControlsEnabled => _areControlsEnabled;

    #region Lifecycle
    private void Awake()
    {
        _gameInputActions = new GameInputActions();
    }

    private void Start()
    {
        InitializeInputActions();

        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _gameInputActions.Enable();
    }

    private void OnDisable()
    {
        _gameInputActions.Disable();
    }
    #endregion

    private void InitializeInputActions ()
    {
        _gameInputActions.Player.Pause.performed += OnPausePerformed;
    }

    // Actions
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        _gameTimer.Pause();

        if (_gameTimer.IsPaused)
            _playerController.DisableInput();
        else
            _playerController.EnableInput();
    }
}