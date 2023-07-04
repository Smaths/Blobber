using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SFXManager
{
    public class SFXManager : MonoBehaviour
    {
        private static SFXManager _instance;

        public static SFXManager Instance
        {
            get
            {
                if (_instance ==null)
                {
                    _instance = FindObjectOfType<SFXManager>();
                }

                return _instance;
            }
        }

        [Title("SFX Manager", "Single location to play all SFX clips needed in scene. ")]
        [HorizontalGroup("AudioSource")]
        [SerializeField]
        private AudioSource _defaultAudioSource;

        [Space]
        [TabGroup("SFX")]
        [AssetList(Path = "/Audio/Game SFX", AutoPopulate = true)]
        public List<SFXClip> gameSFX;
        [TabGroup("UI")]
        [AssetList(Path = "/Audio/UI SFX", AutoPopulate = true)]
        public List<SFXClip> uiSFX;
        [TabGroup("Ambient")]
        [AssetList(Path = "/Audio/Ambient SFX", AutoPopulate = true)]
        public List<SFXClip> ambientSFX;

        public static void PlaySFX(SFXClip sfxClip, bool waitToFinish = true, AudioSource audioSource = null)
        {
            if (audioSource == null)
                audioSource = Instance._defaultAudioSource;

            if (audioSource == null)
            {
                Debug.Log($"You forgot to add a default audio source!");
                return;
            }

            if (!audioSource.isPlaying || !waitToFinish)
            {
                if (sfxClip.clips.Count == 0) return;

                // Get clip
                switch (sfxClip.playType)
                {
                    case SFXClipPlayType.Next:
                        audioSource.clip = sfxClip.NextClip();
                        break;
                    case SFXClipPlayType.Random:
                        audioSource.clip = sfxClip.RandomClip();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Apply variation
                audioSource.volume = sfxClip.volume + Random.Range(-sfxClip.volumeVariation, sfxClip.volumeVariation);
                audioSource.pitch = sfxClip.pitch + Random.Range(-sfxClip.pitchVariation, sfxClip.pitchVariation);

                // Play audio (with delay if present)
                if (sfxClip.delay <= 0)
                    audioSource.Play();
                else
                    audioSource.PlayDelayed(sfxClip.delay);
            }
        }

        [HorizontalGroup("AudioSource")]
        [ShowIf("@_defaultAudioSource == null")]
        [GUIColor(1f, 0.5f, 0.5f)]
        [Button]
        private void AddAudioSource()
        {
            _defaultAudioSource = GetComponent<AudioSource>();

            if (_defaultAudioSource == null)
            {
                _defaultAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public enum SFXType
        {
            SFX,
            UI,
            Ambient,
        }
    }
}