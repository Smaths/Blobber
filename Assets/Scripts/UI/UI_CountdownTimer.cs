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
            if (GameTimer.instance)
            {
                print($"{gameObject.name} - OnEnable");
                GameTimer.instance.OnTimeChanged.AddListener(SetTimeLabel);
            }
        }

        private void OnDisable()
        {
            if (GameTimer.instance)
            {
                print($"{gameObject.name} - OnDisable");
                GameTimer.instance.OnTimeChanged.RemoveListener(SetTimeLabel);
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