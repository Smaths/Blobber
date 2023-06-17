using System;
using System.Globalization;
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
            if (ScoreManager.instance)
            {
                _label.text = ScoreManager.instance.Points.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}