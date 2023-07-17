using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Utility
{
    public class AudioMixerController : Singleton<AudioMixerController>
    {
        [SerializeField] private AudioMixer audioMixer;

        private void Start()
        {
            if (PlayerPrefs.HasKey(PrefKeys.MusicLevel))
                SetMusicVolume(PlayerPrefs.GetFloat(PrefKeys.MusicLevel));

            if (PlayerPrefs.HasKey(PrefKeys.AmbientLevel))
                SetAmbientVolume(PlayerPrefs.GetFloat(PrefKeys.AmbientLevel));

            if (PlayerPrefs.HasKey(PrefKeys.SFXLevel))
                SetSFXVolume(PlayerPrefs.GetFloat(PrefKeys.SFXLevel));

            if (PlayerPrefs.HasKey(PrefKeys.UILevel))
                SetUIVolume(PlayerPrefs.GetFloat(PrefKeys.UILevel));
        }

        public void SetMasterVolume(float sliderValue)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void SetMusicVolume(float sliderValue)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void SetAmbientVolume(float sliderValue)
        {
            audioMixer.SetFloat("AmbientVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void SetSFXVolume(float sliderValue)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void SetUIVolume(float sliderValue)
        {
            audioMixer.SetFloat("UIVolume", Mathf.Log10(sliderValue) * 20);
        }

        // Add more methods to adjust other parameters in the Audio Mixer if needed
    }
}