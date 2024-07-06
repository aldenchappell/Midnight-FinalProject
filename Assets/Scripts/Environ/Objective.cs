using UnityEngine;
using UnityEngine.SceneManagement;

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
        

        if (LevelCompletionManager.Instance.hasCompletedLobby)
        {
            if (SceneManager.GetActiveScene().name == "LOBBY")
            {
                if(!GetComponent<LobbyDoorExit>() && !gameObject.CompareTag("CubbyKey"))
                    CompleteObjective();
            }
        }
        
        if (disableAtRuntime)
        {
            gameObject.SetActive(false);
        }

        if (!LevelCompletionManager.Instance.allLevelsCompleted && GetComponent<LobbyDoorExit>())
        {
            Destroy(GetComponent<Objective>());
        }
    }

    private void Start()
    {
        _objectiveController.RegisterObjective(this);
    }

    public void CompleteObjective()
    {
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