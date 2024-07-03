using System.Linq;
using TMPro;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    [SerializeField] private GameObject taskUI;
    [SerializeField] private Animator taskUIAnimator;
    [SerializeField] private TMP_Text objectiveText;

    private bool isTaskUIOpen = false;
    private void Start()
    {
        //taskUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTaskUIOpen = !isTaskUIOpen;

            if (isTaskUIOpen)
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

        //sort objectives - thank you stack overflow :)
        var sortedObjectives = objectives.OrderBy(o => o.order).ToList();

        foreach (var objective in sortedObjectives)
        {
            if (objective.isCompleted)
            {
                objectiveText.text += $"<s>{objective.description}</s>\n";
            }
            else
            {
                objectiveText.text += $"{objective.description}\n";
            }
        }
    }
}