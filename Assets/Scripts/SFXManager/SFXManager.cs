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

        public static void PlaySFX(SFXClip sfx, bool waitToFinish = true, AudioSource audioSource = null)
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
                switch (sfx.playType)
                {
                    case SFXClipPlayType.Next:
                        audioSource.clip = sfx.NextClip();
                        break;
                    case SFXClipPlayType.Random:
                        audioSource.clip = sfx.RandomClip();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
                audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
                audioSource.Play();
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