using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;


public enum PlayerState { Idle, Moving, Boosting, Attacked, Dead }

public class PlayerController : MonoBehaviour
{
    // Editor fields
    [BoxGroup("Dependencies")]
    [SerializeField] private Renderer _blobRenderer;
    [BoxGroup("Dependencies")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private TMP_Text _nameLabel;

    [Title("Player Settings")]
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 0.15f;
    [Tooltip("Increase or decrease amount when player blob score changes.")]
    [SerializeField] private float _scoreScaleFactor = 0.1f;
    [SerializeField] private float _colorFadeTime = 0.2f;

    [Header("Boost")]
    [SerializeField] private  float boostForce = 10f;          // The force to apply during the boost
    [SerializeField] private  float boostDuration = 1f;        // The duration of the boost
    private float _boostTimer;

    [Header("Info")]
    [SerializeField] [DisplayAsString] private bool _isMoving;
    [SerializeField] [DisplayAsString] private bool _isBoosting;
    [SerializeField] [DisplayAsString] private bool _isGrounded;

    [Header("Faces :D")]
    [SerializeField] private Face _faceData;
    [SerializeField] private Material _faceMaterial;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    [FoldoutGroup("Unity Events", false)]
    public UnityEvent OnScoreDidChange;
    [FoldoutGroup("Unity Events")]
    public UnityEvent OnScoreIncrease;
    [FoldoutGroup("Unity Events")]
    public UnityEvent OnScoreDecrease;
    [FoldoutGroup("Unity Events")]
    public UnityEvent OnDeath;
    [FoldoutGroup("Unity Events")]
    public UnityEvent OnBirth;
    [FoldoutGroup("Unity Events")]
    public UnityEvent OnBoost;

    [Title("WWise Events", "For hooking up player events and stuff to WWise", TitleAlignments.Split)]
    public AK.Wwise.State IsStoppedState;
    public AK.Wwise.State IsMovingState;
    public AK.Wwise.Event IsDeadEvent;
    public AK.Wwise.Event ScoreDecreaseEvent;
    public AK.Wwise.Event ScoreIncreaseEvent;

    // Private fields
    private PlayerState _previousState;
    private CharacterController _controller;
    private Vector2 _moveDirection;
    private Color _cachedColor;

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
                    SetState(PlayerState.Moving);
                    break;
                case false:
                    IsStoppedState.SetValue();
                    SetState(PlayerState.Idle);
                    break;
            }
            _isMoving = value;
        }
    }

    public PlayerState State
    {
        get => _playerState;
    }

    #region Lifecycle
    private void OnValidate()
    {
        _controller ??= GetComponentInChildren<CharacterController>();
        _playerCamera ??= Camera.main;

        if (_blobRenderer)
        {
            if (_faceData != null)
            {
                if (_faceMaterial)
                    SetState(_playerState);
            }
        }
    }

    private void Start()
    {
        _controller ??= GetComponentInChildren<CharacterController>();
        _playerCamera ??= Camera.main;

        IsStoppedState.SetValue();

        _cachedColor = _blobRenderer.material.color;
        _nameLabel.text = LootLockerTool.Instance.PlayerName;

        _isBoosting = false;
        _boostTimer = 0f;

        OnBirth?.Invoke();
    }

    private void Update()
    {
        _isGrounded = _controller.isGrounded;

        transform.position = Vector3.Scale(transform.position, new Vector3(1f, 0f, 1f));    // Keep grounded. 

        MovePlayer();

        if (_isBoosting) BoostPlayer();

        // Check movement for events
        IsMoving = _moveDirection != Vector2.zero;
    }

    private void BoostPlayer()
    {
        if (!_isBoosting) return;   // Guard

        // If the character is boosting, apply the boost force
        Vector3 boostVelocity = transform.forward * boostForce;
        _controller.Move(boostVelocity * Time.deltaTime);

        // Update the boost timer and check if the boost duration has expired
        _boostTimer += Time.deltaTime;
        if (_boostTimer >= boostDuration)
        {
            EndBoost();
        }
    }

    private void OnEnable()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.ScoreChanged.AddListener(ResizeOnScoreDidChange);
            LevelManager.instance.OnScoreIncrease.AddListener(OnScoreDidIncrease);
            LevelManager.instance.OnScoreDecrease.AddListener(OnScoreDidDecrease);
            LevelManager.instance.OnPlayerPointsDepleted.AddListener(OnDidDie);
        }
    }

    private void OnDisable()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.ScoreChanged.RemoveListener(ResizeOnScoreDidChange);
            LevelManager.instance.OnScoreIncrease.RemoveListener(OnScoreDidIncrease);
            LevelManager.instance.OnScoreDecrease.RemoveListener(OnScoreDidDecrease);
            LevelManager.instance.OnPlayerPointsDepleted.RemoveListener(OnDidDie);
        }
    }
    #endregion

    private void SetState(PlayerState newState)
    {
        if (_playerState == newState) return; // ignore redundant state sets

        // Set blob face
        if (_faceMaterial & _faceData)
            UpdateFace(newState);

        _previousState = _playerState;
        _playerState = newState;
    }

    #region Player Input
    public void OnMoveInput(InputAction.CallbackContext context)
    {

        // Convert input to world space relative to the camera orientation
        Vector3 cameraForward = Vector3.Scale(_playerCamera.transform.forward, new Vector3(1, 1, 0)).normalized;
        var moveInput = context.ReadValue<Vector2>();
        _moveDirection = (cameraForward * moveInput.y + _playerCamera.transform.right * moveInput.x).normalized;

    }

    public void OnBoostInput(InputAction.CallbackContext context)
    {
        StartBoost();
    }
    #endregion

    #region Movement
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

    #region Boost
    private void StartBoost()
    {
        _isBoosting = true;
        _boostTimer = 0f;

        SetState(PlayerState.Boosting);

        OnBoost?.Invoke();
    }

    private void EndBoost()
    {
        _isBoosting = false;

        // Perform any necessary actions after the boost has ended
        SetState(_previousState);
    }
    #endregion

    #region Face
    private void UpdateFace(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                SetFace(_faceData.Idleface);
                break;
            case PlayerState.Moving:
                SetFace(_faceData.WalkFace);
                break;
            case PlayerState.Boosting:
                SetFace(_faceData.attackFace);
                break;
            case PlayerState.Attacked:
            case PlayerState.Dead:
                SetFace(_faceData.damageFace);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    private void SetFace(Texture tex)
    {
        _faceMaterial.SetTexture(MainTex, tex);
    }
    #endregion

    #region Subscribed Event Handlers
    public void OnScoreDidIncrease(int amount)
    {
        // Flash color
        _blobRenderer.material.DOColor(Color.yellow, _colorFadeTime).OnComplete(() =>
        {
            _blobRenderer.material.DOBlendableColor(_cachedColor, _colorFadeTime);
        });

        // Events
        OnScoreIncrease?.Invoke();
        ScoreIncreaseEvent.Post(gameObject);
    }

    public void OnScoreDidDecrease(int amount)
    {
        // Flash color
        _blobRenderer.material.DOBlendableColor(Color.red, _colorFadeTime).OnComplete(() =>
        {
            _blobRenderer.material.DOBlendableColor(_cachedColor, _colorFadeTime);
        });

        // Events
        OnScoreDecrease?.Invoke();
        ScoreDecreaseEvent.Post(gameObject);
    }

    public void OnDidDie()
    {
        OnDeath?.Invoke();
        IsDeadEvent.Post(gameObject);
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

        // Event
        OnScoreDidChange?.Invoke();
    }
    #endregion
}
