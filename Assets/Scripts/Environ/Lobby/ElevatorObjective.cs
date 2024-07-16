using UnityEngine;

public class ElevatorObjective : MonoBehaviour
{
    private bool _shouldCheckTrigger;

    private void Start()
    {
        if (LevelCompletionManager.Instance.allLevelsCompleted ||
            LevelCompletionManager.Instance._keysReturned >= 2 && LevelCompletionManager.Instance.hasKey)
        {
            Destroy(gameObject);
        }
        
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
