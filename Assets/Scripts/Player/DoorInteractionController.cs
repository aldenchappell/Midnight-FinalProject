using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionController : MonoBehaviour
{
    private DoorController _currentDoor;

    private PlayerInteractableController _interactableController;
    private void Awake()
    {
        _interactableController = GetComponent<PlayerInteractableController>();
    }

    // private void FindDoor(GameObject door)
    // {
    //     _currentDoor = null;
    //
    //     if (Physics.Raycast(transform.position,
    //             _interactableController.worldInteractionPoint.forward,
    //             out RaycastHit hitInfo,
    //             _interactableController.doorInteractionDistance,
    //             _interactableController.doorObjectMask))
    //     {
    //         if (hitInfo.collider.CompareTag("Door"))
    //         {
    //             _currentDoor = hitInfo.collider.gameObject.GetComponent<DoorController>();
    //
    //             if (Input.GetKeyDown(_interactableController.doorInteractionKey)
    //                 && _currentDoor != null)
    //             {
    //                 if (_currentDoor.canBeOpened)
    //                 {
    //                     _currentDoor.OpenDoor();
    //                     _currentDoor = null;
    //                 }
    //                 else
    //                 {
    //                     _currentDoor.CloseDoor();
    //                     _currentDoor = null;
    //                 }
    //                 
    //             }
    //         }
    //     }
    // }
}
