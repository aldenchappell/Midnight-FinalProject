using System;
using UnityEngine;

public class ElevatorObjective : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(Disable), 1.0f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        GetComponent<Objective>().CompleteObjective();
        Destroy(gameObject);
    }
}
