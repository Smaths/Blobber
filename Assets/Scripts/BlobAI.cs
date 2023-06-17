using UnityEngine;

public class BlobAI : MonoBehaviour
{
    [SerializeField] private int _pointValue = 5;

    private void OnTriggerEnter(Collider other)
    {
        print($"{gameObject.name} - Collision Enter: {other.gameObject.name}");

        ScoreManager.instance.AddPoints(_pointValue);

        Destroy(gameObject);
    }
}