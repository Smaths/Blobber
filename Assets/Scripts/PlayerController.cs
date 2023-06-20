using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    // Editor fields
    [FormerlySerializedAs("_characterController")]
    [BoxGroup("Dependencies")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Renderer _blobRenderer;

    [Title("Player Settings")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 0.15f;
    [Tooltip("Increase or decrease amount when player blob score changes.")]
    [SerializeField] private float _scoreScaleFactor = 0.1f;
    [SerializeField] private float _colorFadeTime = 0.2f;

    [Header("Boost")]
    [SerializeField] private  float boostForce = 10f;          // The force to apply during the boost
    [SerializeField] private  float boostDuration = 1f;        // The duration of the boost
    private bool isBoosting;
    private float boostTimer;

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
        _controller ??= GetComponentInChildren<CharacterController>();
    }

    private void Start()
    {
        IsStoppedState.SetValue();

        isBoosting = false;
        boostTimer = 0f;
    }

    private void Update()
    {
        _isGrounded = _controller.isGrounded;

        MovePlayer();

        if (isBoosting)
        {
            CheckBoost();
        }
        else
        {
            CheckMovement();
        }
    }

    private void CheckBoost()
    {
        // If the character is boosting, apply the boost force
        if (isBoosting)
        {
            Vector3 boostVelocity = transform.forward * boostForce;
            _controller.Move(boostVelocity * Time.deltaTime);

            // Update the boost timer and check if the boost duration has expired
            boostTimer += Time.deltaTime;
            if (boostTimer >= boostDuration)
            {
                EndBoost();
            }
        }
    }

    private void OnEnable()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.ScoreChanged.AddListener(ResizeOnScoreDidChange);
        }
    }

    private void OnDisable()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.ScoreChanged.RemoveListener(ResizeOnScoreDidChange);
        }
    }
    #endregion

    #region Movement
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

    #region Player Input
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        print($"{gameObject.name} - Boost!");
        StartBoost();
    }
    #endregion

    // Move
    private void MovePlayer()
    {
        if (_moveDirection == Vector2.zero) return;

        // Movement
        // Vector3 movement = new Vector3(_moveDirection.x, _characterController.isGrounded ? 0 : -1.0f, _moveDirection.y) * _moveSpeed * Time.deltaTime ;
        Vector3 movement = new Vector3(_moveDirection.x, 0f, _moveDirection.y) * _moveSpeed * Time.deltaTime ;

        // Rotate character to face movement direction
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), _rotationSpeed);

        _controller.Move(movement);
    }
    #endregion

    public void OnScoreDidIncrease()
    {
        Color cachedColor = _blobRenderer.material.color;
        _blobRenderer.material.DOBlendableColor(Color.yellow, _colorFadeTime).OnComplete(() =>
        {
            _blobRenderer.material.DOBlendableColor(cachedColor, _colorFadeTime);
        });

        ScoreIncreaseEvent.Post(gameObject);
    }

    public void OnScoreDidDecrease()
    {
        Color cachedColor = _blobRenderer.material.color;
        _blobRenderer.material.DOBlendableColor(Color.red, _colorFadeTime).OnComplete(() =>
        {
            _blobRenderer.material.DOBlendableColor(cachedColor, _colorFadeTime);
        });

        ScoreDecreaseEvent.Post(gameObject);

        if (LevelManager.instance && LevelManager.instance.Points <= 0)
        {
            IsDeadEvent.Post(gameObject);
        }
    }

    // Resize on score change
    private void ResizeOnScoreDidChange(int amountChanged, int newScore)
    {
        Vector3 currentScale = gameObject.transform.localScale;
        float decreaseFactor = _scoreScaleFactor * 2f;
        Vector3 newScale = amountChanged > 0
            ?
            // Increase size
            new Vector3(currentScale.x + _scoreScaleFactor, currentScale.y + _scoreScaleFactor, currentScale.z + _scoreScaleFactor)
            :
            // Decrease size
            new Vector3(currentScale.x - decreaseFactor, currentScale.y - decreaseFactor, currentScale.z - decreaseFactor);

        gameObject.transform.DOScale(newScale, 0.25f);

        // Increase character controller properties
        float ccSizeScaleAmount = (_scoreScaleFactor / 100f) / 2.0f;
        Vector3 center = _controller.center;
        if (amountChanged <= 0)
        {
            center = new Vector3(
                center.x,
                center.y - (ccSizeScaleAmount * 2f),
                center.z);
            _controller.center = center;

            _controller.radius -= ccSizeScaleAmount;
        }
        else
        {
            center = new Vector3(
                center.x,
                center.y + ccSizeScaleAmount,
                center.z);
            _controller.center = center;

            _controller.radius += ccSizeScaleAmount;
        }
    }

    // Boost
    private void StartBoost()
    {
        isBoosting = true;
        boostTimer = 0f;
    }

    private void EndBoost()
    {
        isBoosting = false;
        // Perform any necessary actions after the boost has ended
    }
}
