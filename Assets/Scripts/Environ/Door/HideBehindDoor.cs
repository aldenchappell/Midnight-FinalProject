using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class HideBehindDoor : MonoBehaviour
{
    private CinemachineVirtualCamera _playerCam;
    [SerializeField] CinemachineVirtualCamera _doorHideCamera;
    [SerializeField] CinemachineVirtualCamera _doorSpyHoleCamera;
    private Camera _mainCamera;

    private DoorController _doorController;
    private GameObject _player;
    private FirstPersonController _FPC;

    private bool _isActive;

    private void Awake()
    {
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        _doorController = GetComponent<DoorController>();
        _player = GameObject.Find("PlayerCapsule");
        _FPC = FindObjectOfType<FirstPersonController>();
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

    #region Change Hide State
    public void StartChangeState()
    {
        StartCoroutine("ChangeHideState");
    }
    private IEnumerator ChangeHideState()
    {
        _isActive = !_isActive;
        HidePlayer();
        if(_isActive)
        {
            yield return new WaitForSeconds(1f);
            _playerCam.Priority = 0;
            _doorHideCamera.Priority = 5;
            _doorController.Invoke("HandleDoor", 2f);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            _playerCam.Priority = 5;
            _doorHideCamera.Priority = 0;
            _doorController.Invoke("HandleDoor", 2f);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
    #endregion

    #region Input and Raycast
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
    #endregion

    #region Peep Hole Camera
    private void SwapDoorCameraPosition()
    {
        if(_doorHideCamera.Priority > 0)
        {
            _doorHideCamera.Priority = 0;
            _doorSpyHoleCamera.Priority = 5;

            //Activate SpyHole Exterior Box Collider to allow the player to return from spy hole camera position
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
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
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
    }

    #endregion

    private void HidePlayer()
    {
        if(_isActive)
        {
            _player.transform.gameObject.layer = 0;
            _FPC.ToggleCanMove();
        }
        else
        {
            _player.transform.gameObject.layer = 6;
            _FPC.ToggleCanMove();
        }
    }
}
