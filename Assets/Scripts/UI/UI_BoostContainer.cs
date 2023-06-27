using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class UI_BoostContainer : MonoBehaviour
    {
        [BoxGroup("Dependencies")]
        [SerializeField] private PlayerController _playerController;

        [SerializeField] private int _currentBoostCount;
        [SerializeField] private UI_Boost[] _boostIndicators;
        private UI_Boost _activeIcon;

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
            }
        }

        private void Update()
        {
            if (_playerController.BoostCount < 3)
            {
                _activeIcon = _boostIndicators[_currentBoostCount];
                _activeIcon.Indicator.fillAmount = _playerController.BoostProgress;
            }
        }
        #endregion

        #region Event Handlers
        public void AddBoostCount(int count)
        {
            _currentBoostCount += count;

            UpdateBoostIndicators();
        }
        #endregion

        // TODO: Super hack, replace with cleaner logic.
        private void UpdateBoostIndicators()
        {
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