using Managers;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UI_CountdownTimer : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _label;

        private void OnEnable()
        {
            if (GameTimer.Instance)
            {
                GameTimer.Instance.OnTimeChanged.AddListener(SetTimeLabel);
            }
        }

        private void OnDisable()
        {
            if (GameTimer.Instance)
            {
                GameTimer.Instance.OnTimeChanged.RemoveListener(SetTimeLabel);
            }
        }

        private void OnValidate()
        {
            _label ??= GetComponent<TMP_Text>();
        }

        public void SetTimeLabel(string timeString)
        {
            _label.text = timeString;
        }
    }
}