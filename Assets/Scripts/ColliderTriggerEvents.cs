using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerEvents : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;
    [SerializeField] private bool _showDebug;

    // public UnityEvent OnTriggerStayEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (_showDebug) print($"{gameObject.name} - Trigger Enter: {other.name}");
        OnTriggerEnterEvent?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_showDebug) print($"{gameObject.name} - Trigger Exit: {other.name}");
        OnTriggerExitEvent?.Invoke();
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     OnTriggerStayEvent?.Invoke();
    // }
}
