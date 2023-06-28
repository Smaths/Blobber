using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_Boost : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _unfilledColor;
        [SerializeField] private Color _fillingColor;
        [SerializeField] private Color _filledColor;

        public Image Indicator => _image;

        #region Lifecycle
        private void OnValidate()
        {
            _image ??= GetComponent<Image>();
        }

        private void Awake()
        {
            _image ??= GetComponent<Image>();
        }
        #endregion

        public void SetUnfilledState()
        {
            _image.color = _unfilledColor;
            _image.fillAmount = 0f;
        }

        public void SetFillingState(float progress)
        {
            _image.color = _fillingColor;
            _image.fillAmount = progress;
        }

        public void SetFilledState()
        {
            _image.color = _filledColor;
            _image.fillAmount = 100f;

            //_image.transform.DOPunchScale(Vector3.one * 0.8f, 0.5f, 3, 0.5f);
        }
    }
}