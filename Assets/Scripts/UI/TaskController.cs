using System.Collections;
using TMPro;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    [SerializeField] private GameObject taskUI;
    [SerializeField] private Animator taskUIAnimator;
    public TMP_Text[] objectiveTexts;

    private bool isTaskUIOpen = false;

    void Start()
    {
        taskUI.SetActive(false);
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
            }
            else
            {
                taskUIAnimator.SetTrigger("Close");
            }
        }
    }
}