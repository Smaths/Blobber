using UnityEngine;
using UnityEngine.Events;

public class TriggerTween : MonoBehaviour
{
    private bool _isAnimating;

    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;
    // public UnityEvent OnTriggerStayEvent;

    private void OnTriggerEnter(Collider other)
    {
        print($"{gameObject.name} - Trigger Enter: {other.name}");
        OnTriggerEnterEvent?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        print($"{gameObject.name} - Trigger Exit: {other.name}");
        OnTriggerExitEvent?.Invoke();
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     OnTriggerStayEvent?.Invoke();
    // }
}
