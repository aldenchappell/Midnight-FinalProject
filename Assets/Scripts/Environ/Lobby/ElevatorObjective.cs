using System;
using UnityEngine;

public class ElevatorObjective : MonoBehaviour
{
    private bool _shouldCheckTrigger;

    private void Awake()
    {
        if (LevelCompletionManager.Instance.allLevelsCompleted)
            Destroy(gameObject);
    }

    private void Start()
    {
        Invoke(nameof(Disable), 1.0f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        _shouldCheckTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_shouldCheckTrigger)
        {
            GetComponent<Objective>().CompleteObjective();
            Destroy(gameObject);
        }
    }
}
