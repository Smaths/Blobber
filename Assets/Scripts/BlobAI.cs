using DG.Tweening;
using UnityEngine;

public class BlobAI : MonoBehaviour
{
    [SerializeField] private int _pointValue = 5;

    private bool _isDestroying;

    private void OnTriggerEnter(Collider other)
    {
        if (_isDestroying) return;  // guard statement

        print(_pointValue > 0
            ? $"{gameObject.name} Collision - Add {_pointValue}"
            : $"{gameObject.name} Collision - Subtract {_pointValue}");

        ScoreManager.instance.AddPoints(_pointValue);

        // Animation
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f, vibrato: 0));
        sequence.Append(transform.DOScale(Vector3.zero, 0.2f));
        sequence.OnStart(() => _isDestroying = true);
        sequence.OnComplete(() => Destroy(gameObject, 0.1f));
        sequence.Play();
    }
}