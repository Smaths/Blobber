using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Editor fields
    [BoxGroup("Dependencies")]
    [SerializeField] private CharacterController _characterController;

    [Title("Player Settings")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 0.15f;
    [Header("Info")]
    [SerializeField] [DisplayAsString] private bool _isMoving;
    [SerializeField] [DisplayAsString] private bool _isGrounded;

    [Header("WWise Events")]
    public AK.Wwise.State IsStoppedState;
    public AK.Wwise.State IsMovingState;

    public AK.Wwise.Event IsDeadEvent;
    public AK.Wwise.Event ScoreDecreaseEvent;
    public AK.Wwise.Event ScoreIncreaseEvent;

    // Private fields
    private Vector2 _moveDirection;
    private bool _isInputDisabled;

    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            if (_isMoving == value) return; // Ignore if value is unchanged.
            switch (value)
            {
                case true:
                    IsMovingState.SetValue();
                    break;
                case false:
                    IsStoppedState.SetValue();
                    break;
            }
            _isMoving = value;
        }
    }

    #region Lifecycle
    private void OnValidate()
    {
        _characterController ??= GetComponentInChildren<CharacterController>();
    }

    private void Start()
    {
        IsStoppedState.SetValue();
    }

    private void Update()
    {
        _isGrounded = _characterController.isGrounded;

        MovePlayer();

        CheckMovement();

        // CheckScoreForSize();
    }

    private void OnEnable()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.ScoreChanged.AddListener(CheckScoreForSize);
        }
    }

    private void OnDisable()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.ScoreChanged.RemoveListener(CheckScoreForSize);
        }
    }

    private void CheckScoreForSize(int newScore)
    {

    }

    private void CheckMovement()
    {
        if (_moveDirection == Vector2.zero)
        {
            IsMoving = false;
        }
        else
        {
            IsMoving = true;
        }
    }
    #endregion

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        if (_isInputDisabled) return;
        if (_moveDirection == Vector2.zero) return;

        // Movement
        // Vector3 movement = new Vector3(_moveDirection.x, _characterController.isGrounded ? 0 : -1.0f, _moveDirection.y) * _moveSpeed * Time.deltaTime ;
        Vector3 movement = new Vector3(_moveDirection.x, 0f, _moveDirection.y) * _moveSpeed * Time.deltaTime ;

        // Rotate character to face movement direction
        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        // Rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), _rotationSpeed);

        _characterController.Move(movement);
    }

    public void DisableInput()
    {
        _isInputDisabled = true;
    }

    public void EnableInput()
    {
        _isInputDisabled = false;
    }
}
