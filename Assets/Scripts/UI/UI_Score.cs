using System.Globalization;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_Score : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int _score;
        [SerializeField] private TMP_Text _label;

        private void OnValidate()
        {
            SetScoreLabel();
        }

        private void Start()
        {
            SetScoreLabel();
        }

        private void Update()
        {
            SetScoreLabel();
        }

        // Private methods
        private void SetScoreLabel()
        {
            if (ScoreManager.Instance)
            {
                _label.text = ScoreManager.Instance.Points.ToString(CultureInfo.CurrentCulture);
            }
        }
    }
}