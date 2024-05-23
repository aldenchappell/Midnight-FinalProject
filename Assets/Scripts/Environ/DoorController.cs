using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool canBeOpened = true;

    [SerializeField] private float doorInteractionCooldown = 1.5f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void HandleDoor()
    {
        if (canBeOpened)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }
    
    private void OpenDoor()
    {
        _animator.SetTrigger("Open");
        StartDoorInteractionCooldown();

        Debug.Log("opening door");
    }

    private void CloseDoor()
    {
        _animator.SetTrigger("Close");
        StartDoorInteractionCooldown();
        
        Debug.Log("closing door");
    }

    private void StartDoorInteractionCooldown()
    {
        StartCoroutine(InitiateDoorInteractionCooldown());
    }

    private IEnumerator InitiateDoorInteractionCooldown()
    {
        canBeOpened = false;
        yield return new WaitForSeconds(doorInteractionCooldown);
        canBeOpened = true;
    }
}
