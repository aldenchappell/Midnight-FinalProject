using UnityEngine;

public class Objective : MonoBehaviour
{
    public string description;
    public bool isCompleted = false;
    public int order;
    public bool disableAtRuntime;
    
    private ObjectiveController _objectiveController;

    private void Awake()
    {
        _objectiveController = FindObjectOfType<ObjectiveController>();
        
        if (disableAtRuntime)
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if(!GetComponent<LobbyDoorExit>())
            _objectiveController.RegisterObjective(this);

        if (GetComponent<LobbyDoorExit>() && LevelCompletionManager.Instance.allLevelsCompleted)
        {
            _objectiveController.RegisterObjective(this);
            GetComponent<LobbyDoorExit>().particles.SetActive(true);
        }
    }

    public void CompleteObjective()
    {
        bool isLobbyDoor = GetComponent<LobbyDoorExit>() && !LevelCompletionManager .Instance.allLevelsCompleted;

        if (isLobbyDoor && LevelCompletionManager.Instance._keysReturned < 3) return;
        
        isCompleted = true;
        if (_objectiveController != null)
        {
            _objectiveController.UpdateTaskList();
            EnvironmentalSoundController.Instance.PlaySound(_objectiveController.pencilSound, transform.position);
        }
        else
        {
            Debug.LogWarning("ObjectiveController is null when trying to complete objective.");
        }
    }
}
