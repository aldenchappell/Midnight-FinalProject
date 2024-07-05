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
        _objectiveController.RegisterObjective(this);

        if (PlayerPrefs.GetInt("LobbyPowered", 0) == 1)
        {
            if (SceneManager.GetActiveScene().name == "LOBBY")
            {
                CompleteObjective();
            }
        }
        
        if (disableAtRuntime)
        {
            gameObject.SetActive(false);
        }
    }

    public void CompleteObjective()
    {
        isCompleted = true;
        if (_objectiveController != null)
        {
            _objectiveController.UpdateTaskList();
            EnvironmentalSoundController.Instance.PlaySound(_objectiveController.pencilSound, transform.position);
            //Debug.Log("Finished objective " + description);
        }
        else
        {
            Debug.LogWarning("ObjectiveController is null when trying to complete objective.");
        }
    }
}