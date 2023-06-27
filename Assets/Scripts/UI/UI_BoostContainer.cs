using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UI_BoostContainer : MonoBehaviour
    {
        [BoxGroup("Dependencies")]
        [SerializeField] private PlayerController _playerController;

        [SerializeField] private int _currentBoostCount;
        [SerializeField] private UI_Boost[] _boostIndicators;
        private UI_Boost _activeIcon;

        public UnityEvent OnBoostTapped;

        #region Lifecycle
        private void OnValidate()
        {
            _boostIndicators = GetComponentsInChildren<UI_Boost>();

            UpdateBoostIndicators();
        }

        private void OnEnable()
        {
            _boostIndicators ??= GetComponentsInChildren<UI_Boost>();

            UpdateBoostIndicators();
        }

        private void Start()
        {
            if (_playerController)
            {
                _currentBoostCount = _playerController.BoostCount;
                UpdateBoostIndicators();
            }
        }

        private void Update()
        {
            if (_playerController.BoostCount < 3)
            {
                _activeIcon = _boostIndicators[_currentBoostCount];
                _activeIcon.SetFillingState(_playerController.BoostProgress);
            }
        }
        #endregion

        #region Event Handlers
        public void AddBoostCount(int count)
        {
            _currentBoostCount += count;

            UpdateBoostIndicators();
        }

        public void Boost_Tapped()
        {
            OnBoostTapped?.Invoke();
        }
        #endregion

        private void UpdateBoostIndicators()
        {
            // TODO: Super hack, replace with cleaner logic.
            switch (_currentBoostCount)
            {
                case 0:
                    _boostIndicators[0].SetUnfilledState();
                    _boostIndicators[1].SetUnfilledState();
                    _boostIndicators[2].SetUnfilledState();
                    break;
                case 1:
                    _boostIndicators[0].SetFilledState();
                    _boostIndicators[1].SetUnfilledState();
                    _boostIndicators[2].SetUnfilledState();
                    break;
                case 2:
                    _boostIndicators[0].SetFilledState();
                    _boostIndicators[1].SetFilledState();
                    _boostIndicators[2].SetUnfilledState();
                    break;
                case 3:
                    _boostIndicators[0].SetFilledState();
                    _boostIndicators[1].SetFilledState();
                    _boostIndicators[2].SetFilledState();
                    break;
            }
        }
    }
}