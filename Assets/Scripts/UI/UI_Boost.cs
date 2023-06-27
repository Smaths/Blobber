using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum BoostIndicatorState
    {
        Unfilled,
        Filling,
        Filled
    }

    public class UI_Boost : MonoBehaviour
    {
        [SerializeField] private Color _unfilledColor;
        [SerializeField] private Color _fillingColor;
        [SerializeField] private Color _filledColor;

        private Image _image;
        public Image Indicator => _image;

        #region Lifecycle
        private void OnValidate()
        {
            _image ??= GetComponentInChildren<Image>();
        }

        private void Awake()
        {
            _image ??= GetComponentInChildren<Image>();
            _image ??= GetComponentInChildren<Image>();
        }
        #endregion

        private void SetState(BoostIndicatorState newState)
        {
            switch (newState)
            {
                case BoostIndicatorState.Unfilled:
                    SetUnfilledState();
                    break;
                case BoostIndicatorState.Filling:
                    SetFillingState(0f);
                    break;
                case BoostIndicatorState.Filled:
                    SetFilledState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public void SetUnfilledState()
        {
            _image ??= GetComponentInChildren<Image>();
            _image.color = _unfilledColor;
            _image.fillAmount = 0f;
        }

        public void SetFillingState(float progress)
        {
            _image ??= GetComponentInChildren<Image>();
            _image.color = _fillingColor;
        }

        public void SetFilledState()
        {
            _image ??= GetComponentInChildren<Image>();
            _image.color = _filledColor;
            _image.fillAmount = 100f;
        }
    }
}