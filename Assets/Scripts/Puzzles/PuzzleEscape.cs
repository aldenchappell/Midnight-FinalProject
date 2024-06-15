using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEscape : MonoBehaviour
{
    public UnityEvent EscapePressed;
    private bool isActive;
    private PauseManager _pauseManager;

    private void Awake()
    {
        _pauseManager = GameObject.FindFirstObjectByType<PauseManager>();
    }

    private void Update()
    {
        if(isActive)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EscapePressed.Invoke();
            ChangeIsActive();
            isActive = false;
        }
    }
    
    public void ChangeIsActive()
    {
        isActive = !isActive;
        if(isActive)
        {
            GetComponent<InteractableObject>().enabled = false;
            _pauseManager.SetPuzzleBool = true;
        }
        else
        {
            GetComponent<InteractableObject>().enabled = true;
            _pauseManager.SetPuzzleBool = false;
        }
    }
}
