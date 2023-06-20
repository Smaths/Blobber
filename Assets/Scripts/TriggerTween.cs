using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class TriggerTween : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation[] _tweenAnimations;
    private bool _isAnimating;

    private void OnValidate()
    {
        FindTweenAnimations();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_tweenAnimations.IsNullOrEmpty())
        {
            foreach (var tweenAnimation in _tweenAnimations)
            {
                tweenAnimation.DORestart();
            }
        }
    }

    [Button]
    private void FindTweenAnimations()
    {
        _tweenAnimations = null;
        _tweenAnimations = GetComponentsInChildren<DOTweenAnimation>();
    }
}
