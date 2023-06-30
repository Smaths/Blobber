using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFXManager
{
    public enum SFXClipPlayType { Next, Random }

    [CreateAssetMenu(fileName = "New SFX Clip", menuName = "NewSFXClip", order = 0)]
    public class SFXClip : ScriptableObject
    {
        [Space]
        [Title("Audio Clip")]
        [Required]
        public List<AudioClip> clips;
        public SFXClipPlayType playType;

        [Title("Clip Settings")]
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0f, 0.2f)]
        public float volumeVariation = 0.05f;
        [Range(0f, 2f)]
        public float pitch = 1f;
        [Range(0f, 0.2f)]
        public float pitchVariation = 0.05f;
        [MinValue(0)]
        [SuffixLabel("second(s)")]
        public float delay = 0f;

        private int _currentIndex;

        public AudioClip NextClip()
        {
            if (clips.Count <= 0)
            {
                Debug.Log($"{name} has no audio clips!");
                return null;
            }

            // Increment current index but avoid going out of bounds.
            if (_currentIndex < clips.Count-1) _currentIndex++;
            else _currentIndex = 0;  // reset index

            AudioClip nextClip = clips[_currentIndex];

            return nextClip;
        }

        public AudioClip  RandomClip()
        {
            if (clips.Count <= 0)
            {
                Debug.Log($"{name} has no audio clips!");
                return null;
            }

            // Get random index make sure it doesn't match previous clip (for better variety).
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, clips.Count-1);
            }
            while (randomIndex == _currentIndex);

            _currentIndex = randomIndex;
            AudioClip randomClip = clips[randomIndex];
            return randomClip;
        }
    }
}