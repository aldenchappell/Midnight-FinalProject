using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HideBehindDoor : MonoBehaviour
{
    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _doorHideCamera;
    private CinemachineVirtualCamera _doorSpyHoleCamera;
    private Camera _mainCamera;

    private DoorController _doorController;

    private bool _isActive;

    private void Awake()
    {
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _doorHideCamera = gameObject.transform.parent.GetChild(2).GetComponent<CinemachineVirtualCamera>();
        _mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        _doorSpyHoleCamera = gameObject.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();

        _doorController = GetComponent<DoorController>();
    }

    private void Start()
    {
        _isActive = false;
    }

    private void Update()
    {
        if(_isActive)
        {
            CheckForInput();
        }
    }

    public void ChangeHideState()
    {
        _isActive = !_isActive;
        if(_isActive)
        {
            _playerCam.Priority = 0;
            _doorHideCamera.Priority = 5;
            _doorController.Invoke("HandleDoor", 2f);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            _playerCam.Priority = 5;
            _doorHideCamera.Priority = 0;
            _doorController.Invoke("HandleDoor", 2f);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    private void CheckForInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastToMousePosition();
        }
    }

    private void RaycastToMousePosition()
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("SpyHole"))
            {
                SwapDoorCameraPosition();
            }
        }
    }

    private void SwapDoorCameraPosition()
    {
        if(_doorHideCamera.Priority > 0)
        {
            _doorHideCamera.Priority = 0;
            _doorSpyHoleCamera.Priority = 5;

            //Activate SpyHole Exterior Box Collider to allow the player to return from spy hole camera position
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponents<Collider>();
            foreach(Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
        else
        {
            _doorSpyHoleCamera.Priority = 0;
            _doorHideCamera.Priority = 5;

            //Deactivate SpyHole Exterior Box Collider to prevent confusing collision outside of the door
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
    }
}
