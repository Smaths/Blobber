using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;

namespace Blobs {
    public enum PlayerState { Idle, Moving, Boosting, Attacked, Dead }

    public class PlayerController : MonoBehaviour
    {
        // Editor fields
        [BoxGroup("Dependencies")]
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private GameObject _mesh;

        [Title("Player Settings")]
        [SerializeField] private PlayerState _playerState;
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 0.15f;
        [Tooltip("Increase or decrease amount when player blob score changes.")]
        [SerializeField] private float _scoreScaleFactor = 0.1f;
        [SerializeField] private float _colorFadeTime = 0.2f;

        [Header("Boost")]
        [SerializeField] private  float boostForce = 10f;          // The force to apply during the boost
        [SuffixLabel("second(s)"), MinValue(0)]
        [SerializeField] private  float boostDuration = 1f;        // The duration of the boost
        private float _boostTimer;
        [SuffixLabel("second(s)"), MinValue(0)]
        [SerializeField] private float _boostChargeTime = 10f;
        private float _boostChargeTimeTrigger;
        [SerializeField] private int _currentBoostCount;
        [SerializeField] private int _maxBoostCount = 3;

        [Header("Info")]
        [SerializeField] [DisplayAsString] private bool _isMoving;
        [SerializeField] [DisplayAsString] private bool _isBoosting;

        [Header("Faces :D")]
        [SerializeField] private global::Plugins.Kawaii_Slimes.Scripts.AI.Face _faceData;
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
        public UnityEvent OnBoostActivated;
        [FoldoutGroup("Unity Events")]
        public UnityEvent OnBoostChargeAdded;

        // Private fields
        private Camera _playerCamera;
        private PlayerState _previousState;
        private CharacterController _controller;
        private Renderer _blobRenderer;
        private Vector2 _moveDirection;
        private Color _cachedColor;

        #region Public Properties
        public PlayerState State => _playerState;
        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                if (_isMoving == value) return; // Ignore if value is unchanged.
                switch (value)
                {
                    case true:
                        SetState(PlayerState.Moving);
                        break;
                    case false:
                        SetState(PlayerState.Idle);
                        break;
                }
                _isMoving = value;
            }
        }
        public int BoostCount => _currentBoostCount;
        [ShowInInspector]
        public float BoostProgress
        {
            get
            {
                if (BoostChargeTimeRemaining <= 0) return 1.0f;    // No time remaining, progress complete.
                return 1 - BoostChargeTimeRemaining / _boostChargeTime;
            }
        }
        [ShowInInspector]
        public float BoostChargeTimeRemaining => Mathf.Max(_boostChargeTimeTrigger - Time.time, 0);   // NOTE: `Mathf.Max` keeps values above 0.

        public int MaxBoostCount => _maxBoostCount;
        #endregion

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

        private void Awake()
        {
            _controller ??= GetComponentInChildren<CharacterController>();
            _blobRenderer = _mesh.GetComponent<Renderer>();
            _cachedColor = _blobRenderer.material.color;
        }

        private void Start()
        {
            _playerCamera ??= Camera.main;

            _nameLabel.text = LootLockerTool.Instance.PlayerName;

            _isBoosting = false;
            _boostTimer = 0f;

            _boostChargeTimeTrigger = Time.time + _boostChargeTime;

            OnBirth?.Invoke();
        }

        private void Update()
        {
            MovePlayer();

            if (_isBoosting) BoostPlayer();
            else CheckBoost();

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
            if (ScoreManager.instanceExists)
            {
                ScoreManager.Instance.ScoreChanged.AddListener(ResizeOnScoreDidChange);
                ScoreManager.Instance.OnScoreIncrease.AddListener(OnScoreDidIncrease);
                ScoreManager.Instance.OnScoreDecrease.AddListener(OnScoreDidDecrease);
                ScoreManager.Instance.OnScoreIsZero.AddListener(OnDidDie);
            }
        }

        private void OnDisable()
        {
            if (ScoreManager.instanceExists)
            {
                ScoreManager.Instance.ScoreChanged.RemoveListener(ResizeOnScoreDidChange);
                ScoreManager.Instance.OnScoreIncrease.RemoveListener(OnScoreDidIncrease);
                ScoreManager.Instance.OnScoreDecrease.RemoveListener(OnScoreDidDecrease);
                ScoreManager.Instance.OnScoreIsZero.RemoveListener(OnDidDie);
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
        public void OnMovePerformed(InputAction.CallbackContext context)
        {
            // // Convert input to world space relative to the camera orientation
            // Vector3 cameraForward = Vector3.Scale(_playerCamera.transform.forward, new Vector3(1, 1, 0)).normalized;
            // var moveInput = context.ReadValue<Vector2>();
            // _moveDirection = (cameraForward * moveInput.y + _playerCamera.transform.right * moveInput.x).normalized;

            _moveDirection = context.ReadValue<Vector2>().normalized;
        }

        public void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveDirection = Vector2.zero;
        }

        public void OnBoostInput(InputAction.CallbackContext context)
        {
            Boost();
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
        private void CheckBoost()
        {
            if (Time.time >= _boostChargeTimeTrigger)
            {
                // Timer is done
                if (_currentBoostCount >= _maxBoostCount) return;

                _currentBoostCount++;

                RestartBoostTimer();

                OnBoostChargeAdded?.Invoke();
            }

            // Timer still running.
        }

        public void Boost()
        {
            if (_isBoosting) return;
            if (_currentBoostCount == 0) return;

            if (_currentBoostCount >= _maxBoostCount)
                RestartBoostTimer();

            StartBoost();
        }

        private void StartBoost()
        {
            _isBoosting = true;
            _boostTimer = 0f;
            _currentBoostCount--;

            SetState(PlayerState.Boosting);

            OnBoostActivated?.Invoke();
        }

        private void EndBoost()
        {
            _isBoosting = false;

            // Perform any necessary actions after the boost has ended
            SetState(_previousState);
        }

        private void RestartBoostTimer()
        {
            _boostChargeTimeTrigger = Time.time + _boostChargeTime;
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
        }

        public void OnDidDie()
        {
            OnDeath?.Invoke();
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
}