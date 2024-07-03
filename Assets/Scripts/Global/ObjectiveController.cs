using System.Collections.Generic;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    private TaskController taskController;
    private List<Objective> objectives = new List<Objective>();

    private void Start()
    {
        taskController = FindObjectOfType<TaskController>();
        if (taskController == null)
        {
            Debug.LogError("TaskController not found in the scene.");
        }
        UpdateTaskList();
    }

    public void RegisterObjective(Objective objective)
    {
        objectives.Add(objective);
    }

    public void UpdateTaskList()
    {
        taskController.UpdateObjectiveText(objectives.ToArray());
    }
}