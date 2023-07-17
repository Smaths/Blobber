using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_Settings : MonoBehaviour
    {
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private Slider _musicSlider;
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private Slider _ambientSlider;
        [Required] [ChildGameObjectsOnly] [LabelText("SFX Slider")]
        [SerializeField] private Slider _sfxSlider;
        [Required] [ChildGameObjectsOnly] [LabelText("UI Slider")]
        [SerializeField] private Slider _uiSlider;
        [Space]
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private Toggle _kidFriendlyToggle;

        private void OnEnable()
        {
            // Control callback setup
            _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            _ambientSlider.onValueChanged.AddListener(OnAmbientSliderChanged);
            _sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
            _uiSlider.onValueChanged.AddListener(OnUISliderValueChanged);
            _kidFriendlyToggle.onValueChanged.AddListener(OnToggleSwitched);

            // Load from preferences.
            if (PlayerPrefs.HasKey(PrefKeys.MusicLevel)) _musicSlider.value = PlayerPrefs.GetFloat(PrefKeys.MusicLevel);
            else _musicSlider.value = 1;

            if (PlayerPrefs.HasKey(PrefKeys.AmbientLevel)) _ambientSlider.value = PlayerPrefs.GetFloat(PrefKeys.AmbientLevel);
            else _ambientSlider.value = 1;

            if (PlayerPrefs.HasKey(PrefKeys.SFXLevel)) _sfxSlider.value = PlayerPrefs.GetFloat(PrefKeys.SFXLevel);
            else _sfxSlider.value = 1;

            if (PlayerPrefs.HasKey(PrefKeys.UILevel)) _uiSlider.value = PlayerPrefs.GetFloat(PrefKeys.UILevel);
            else _uiSlider.value = 1;

            if (PlayerPrefs.HasKey(PrefKeys.IsInKidMode))
                _kidFriendlyToggle.isOn = PlayerPrefs.GetInt(PrefKeys.IsInKidMode) == 1;

            EventSystem.current.SetSelectedGameObject(_musicSlider.gameObject);
        }

        private void OnDisable()
        {
            _musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
            _ambientSlider.onValueChanged.RemoveListener(OnAmbientSliderChanged);
            _sfxSlider.onValueChanged.RemoveListener(OnSFXSliderValueChanged);
            _uiSlider.onValueChanged.RemoveListener(OnUISliderValueChanged);
            _kidFriendlyToggle.onValueChanged.RemoveListener(OnToggleSwitched);

            PlayerPrefs.SetFloat(PrefKeys.MusicLevel, _musicSlider.value);
            PlayerPrefs.SetFloat(PrefKeys.AmbientLevel, _ambientSlider.value);
            PlayerPrefs.SetFloat(PrefKeys.SFXLevel, _sfxSlider.value);
            PlayerPrefs.SetFloat(PrefKeys.UILevel, _uiSlider.value);
            PlayerPrefs.SetInt(PrefKeys.IsInKidMode, _kidFriendlyToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        private static void OnMusicSliderValueChanged(float value)
        {
            if (AudioMixerController.instanceExists)
            {
                AudioMixerController.Instance.SetMusicVolume(value);
            }
        }

        private static void OnAmbientSliderChanged(float value)
        {
            if (AudioMixerController.instanceExists)
            {
                AudioMixerController.Instance.SetAmbientVolume(value);
            }
        }

        private static void OnSFXSliderValueChanged(float value)
        {
            if (AudioMixerController.instanceExists)
            {
                AudioMixerController.Instance.SetSFXVolume(value);
            }
        }

        private static void OnUISliderValueChanged(float value)
        {
            if (AudioMixerController.instanceExists)
            {
                AudioMixerController.Instance.SetUIVolume(value);
            }
        }

        private static void OnToggleSwitched(bool isOn)
        {
            PlayerPrefs.SetInt(PrefKeys.IsInKidMode, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}