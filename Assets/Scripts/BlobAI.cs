using UnityEngine;

public class BlobAI : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        print($"{gameObject.name} - Collision Enter: {other.gameObject.name}");
    }
}
