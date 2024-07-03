using UnityEngine;

public class Objective : MonoBehaviour
{
    public string description;
    public bool isCompleted = false;
    public int order;

    private ObjectiveController objectiveController;

    private void Start()
    {
        objectiveController = FindObjectOfType<ObjectiveController>();
        if (objectiveController == null)
        {
            Debug.LogError("ObjectiveController not found in the scene.");
        }
        else
        {
            objectiveController.RegisterObjective(this);
        }
    }

    public void CompleteObjective()
    {
        isCompleted = true;
        objectiveController.UpdateTaskList();
        
        EnvironmentalSoundController.Instance.PlaySound(objectiveController.pencilSound,transform.position);
    }
}