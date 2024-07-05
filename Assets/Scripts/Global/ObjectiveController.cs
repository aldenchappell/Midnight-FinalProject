using System.Collections.Generic;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    private TaskController _taskController;
    private List<Objective> _objectives = new List<Objective>();
    public AudioClip pencilSound;

    private void Awake()
    {
        _taskController = FindObjectOfType<TaskController>();
        if (_taskController == null)
        {
            Debug.LogError("TaskController not found in the scene.");
        }
        else
        {
            UpdateTaskList();
        }
    }

    public void RegisterObjective(Objective objective)
    {
        _objectives.Add(objective);
        UpdateTaskList();
    }

    public void UpdateTaskList()
    {
        _taskController.UpdateObjectiveText(_objectives.ToArray());
    }
}