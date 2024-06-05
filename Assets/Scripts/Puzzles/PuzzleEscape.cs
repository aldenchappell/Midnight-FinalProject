using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEscape : MonoBehaviour
{
    public UnityEvent EscapePressed;
    private bool isActive;

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
            isActive = false;
        }
    }
    
    public void ChangeIsActive()
    {
        isActive = true;
    }
}
