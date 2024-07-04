using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    [SerializeField] private GameObject taskUI;
    [SerializeField] private Animator taskUIAnimator;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private AudioSource canvasSource;
    [SerializeField] private AudioClip taskSFX;

    private bool _isTaskUIOpen = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isTaskUIOpen = !_isTaskUIOpen;

            canvasSource.PlayOneShot(taskSFX);
            
            if (_isTaskUIOpen)
            {
                taskUI.SetActive(true);
                taskUIAnimator.SetTrigger("Popout");
                taskUIAnimator.ResetTrigger("Close");
            }
            else
            {
                taskUIAnimator.SetTrigger("Close");
                taskUIAnimator.ResetTrigger("Popout");
            }
        }
    }

    public void UpdateObjectiveText(Objective[] objectives)
    {
        objectiveText.text = "";
        
        //ty stackoverflow :) 
        var sortedObjectives = objectives.OrderBy(o => o.order).ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var objective in sortedObjectives)
        {
            if (objective.isCompleted)
            {
                sb.AppendLine($"<s>{objective.description}</s>");
            }
            else
            {
                sb.AppendLine($"{objective.description}");
            }
        }
        
        objectiveText.text = sb.ToString();
    }

}