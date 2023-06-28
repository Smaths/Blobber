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
        [SerializeField] private UI_Boost[] _indicators;
        private UI_Boost _activeIndicator;

        public UnityEvent OnBoostTapped;

        #region Lifecycle
        private void OnValidate()
        {
            if (Application.isEditor == false) return;

            _indicators ??= GetComponentsInChildren<UI_Boost>();

            SetBoostIndicators(_currentBoostCount);
        }

        private void Start()
        {
            if (_playerController)
            {
                _currentBoostCount = _playerController.BoostCount;
                SetBoostIndicators(_playerController.BoostCount);
            }
        }

        private void Update()
        {
            if (_playerController == null) return;
            // Don't do any work if player boost charges are at max count.
            if (_playerController.BoostCount >= _playerController.MaxBoostCount) return;
            if (_activeIndicator == null) return;

            _activeIndicator.SetFillingState(_playerController.BoostProgress);
        }
        #endregion

        #region Event Handlers
        public void AddBoostCount(int count)
        {
            _currentBoostCount += count;
            SetBoostIndicators(_currentBoostCount);
        }

        public void Boost_Tapped()
        {
            OnBoostTapped?.Invoke();
        }
        #endregion

        private void SetBoostIndicators(int activeCount)
        {
            for (int i = 0; i < _indicators.Length; i++)
            {
                if (i < activeCount) _indicators[i].SetFilledState();
                else _indicators[i].SetUnfilledState();
            }

            if (activeCount >= _indicators.Length) _activeIndicator = null;
            else _activeIndicator = _indicators[activeCount];
        }
    }
}