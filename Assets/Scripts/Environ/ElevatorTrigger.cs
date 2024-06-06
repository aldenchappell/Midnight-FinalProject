using UnityEngine;
using UnityEngine.Events;

public class ElevatorTrigger : MonoBehaviour
{
    public UnityEvent onElevatorTrigger;
    public UnityEvent onElevatorTriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        onElevatorTrigger.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        onElevatorTriggerExit.Invoke();
    }
}
