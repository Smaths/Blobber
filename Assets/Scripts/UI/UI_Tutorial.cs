using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_Tutorial : MonoBehaviour
    {
        [SerializeField] private TMP_Text _goodBlobLabel;
        [SerializeField] private string _normalText = "Gives Points*";
        [SerializeField] private string _transformedText = "Takes Points*";
        [SerializeField] private float _transformEffectTime = 0.4f;

        private bool _transformedTextIsVisible;

        public void OnGoodBlobTransformed()
        {
            _transformedTextIsVisible = !_transformedTextIsVisible;

            if (_transformedTextIsVisible)
            {
                _goodBlobLabel.DOText(_transformedText, _transformEffectTime);
                _goodBlobLabel.DOBlendableColor(ColorConstants.Red, _transformEffectTime);
            }
            else
            {
                _goodBlobLabel.DOText(_normalText, _transformEffectTime);
                _goodBlobLabel.DOBlendableColor(ColorConstants.Yellow, _transformEffectTime);
            }
        }
    }
}