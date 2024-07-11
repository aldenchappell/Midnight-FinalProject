using UnityEngine;

public class ElevatorObjective : MonoBehaviour
{
    private bool shouldCheckTrigger = false;

    private void Awake()
    {
        if (LevelCompletionManager.Instance._keysReturned >= 2 && LevelCompletionManager.Instance.hasKey)
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        Invoke(nameof(Disable), 1.0f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        shouldCheckTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (shouldCheckTrigger)
        {
            GetComponent<Objective>().CompleteObjective();
            Destroy(gameObject);
        }
    }
}
