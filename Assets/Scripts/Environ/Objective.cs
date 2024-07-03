using UnityEngine;

public class Objective : MonoBehaviour
{
    public string description;
    public bool isCompleted = false;
    public int order;

    private ObjectiveController _objectiveController;

    private void Start()
    {
        _objectiveController = FindObjectOfType<ObjectiveController>();
        if (_objectiveController == null)
        {
            Debug.LogError("ObjectiveController not found in the scene.");
        }
        else
        {
            //Debug.Log("Registering objective " + description);
            _objectiveController.RegisterObjective(this);
            _objectiveController.UpdateTaskList();
        }
    }

    public void CompleteObjective()
    {
        isCompleted = true;
        _objectiveController.UpdateTaskList();
        
        EnvironmentalSoundController.Instance.PlaySound(_objectiveController.pencilSound, transform.position);
    }
}