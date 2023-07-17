using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_KidFriendlyToggle : MonoBehaviour
    {
        [Required, ChildGameObjectsOnly]
        [SerializeField] private Toggle _toggle;

        public UnityEvent<bool> ToggleDidSwitch;

        private void OnValidate()
        {
            _toggle = GetComponentInChildren<Toggle>();
        }

        private void OnEnable()
        {
            if (PlayerPrefs.HasKey(PrefKeys.IsInKidMode))
                _toggle.isOn = PlayerPrefs.GetInt(PrefKeys.IsInKidMode) == 1;

            _toggle.onValueChanged.AddListener(OnToggleSwitched);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleSwitched);

            PlayerPrefs.SetInt(PrefKeys.IsInKidMode, _toggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnToggleSwitched(bool isOn)
        {
            PlayerPrefs.SetInt(PrefKeys.IsInKidMode, isOn ? 1 : 0);
            PlayerPrefs.Save();

            ToggleDidSwitch?.Invoke(isOn);
        }
    }
}