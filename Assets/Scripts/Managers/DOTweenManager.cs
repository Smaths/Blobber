using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class DOTweenManager : MonoBehaviour
    {
        [SerializeField] private DOTweenAnimation[] _doTweenAnimations;

        private void Awake()
        {
            FindAnimationsInScene();
        }

        private void Start()
        {
            DOTween.SetTweensCapacity(200, 200);
        }

        #region Public Methods
        public void PauseAnimations()
        {
            foreach (var tween in _doTweenAnimations)
            {
                if (tween == null) continue;
                tween.DOPause();
            }
        }

        public void ResumeAnimations()
        {
            foreach (var tween in _doTweenAnimations)
            {
                if (tween == null) continue;
                tween.DOPlay();
            }
        }
        #endregion

        [Button]
        private void FindAnimationsInScene()
        {
            _doTweenAnimations = FindObjectsOfType<DOTweenAnimation>().ToArray();
        }
    }
}