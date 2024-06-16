using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class HideBehindDoor : MonoBehaviour
{
    int hiddenLayer;
    int defaultLayer;

    private CinemachineVirtualCamera _playerCam;
    [SerializeField] CinemachineVirtualCamera _doorHideCamera;
    [SerializeField] CinemachineVirtualCamera _doorSpyHoleCamera;
    private Camera _mainCamera;

    private DoorController _doorController;
    private GameObject _player;
    private FirstPersonController _FPC;
    private PlayerDualHandInventory _inventory;

    private bool _isActive;
    private bool _isSwitching;

    private void Awake()
    {
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        _doorController = GetComponent<DoorController>();
        _player = GameObject.Find("Player");
        _FPC = FindObjectOfType<FirstPersonController>();

        hiddenLayer = LayerMask.NameToLayer("Default");
        defaultLayer = LayerMask.NameToLayer("Target");

        _inventory = GameObject.FindAnyObjectByType<PlayerDualHandInventory>();
    }

    private void Start()
    {
        _isActive = false;
        _isSwitching = false;
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
        if(!_isSwitching)
        {
            StartCoroutine("ChangeHideState");
        }
    }
    private IEnumerator ChangeHideState()
    {
        _isActive = !_isActive;
        _isSwitching = true;
        HidePlayer();
        if(_isActive)
        {
            _doorController.Invoke("HandleDoor", 0);
            yield return new WaitForSeconds(1f);
            _playerCam.Priority = 0;
            _doorHideCamera.Priority = 5;
            _doorController.Invoke("HandleDoor", 2f);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
        }
        else
        {
            _doorController.Invoke("HandleDoor", 0);
            yield return new WaitForSeconds(1f);
            _playerCam.Priority = 5;
            _doorHideCamera.Priority = 0;
            _doorController.Invoke("HandleDoor", 2f);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
        }
        Invoke("InteractDelay", 4f);
        

    }

    private void InteractDelay()
    {
        print("Interaction is availabe");
        _isSwitching = false;
    }

    #endregion

    #region Input and Raycast
    private void CheckForInput()
    {
        if(Input.GetMouseButtonDown(0) && _isSwitching == false)
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
                _isSwitching = true;
                SwapDoorCameraPosition();
                Invoke("InteractDelay", 1f);
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
            _player.layer = hiddenLayer;
            _FPC.ToggleCanMove();
            //Hide items to prevent them from being seen outside of door
            _inventory.HideHandItem();
        }
        else
        {
            _player.layer = defaultLayer;
            _FPC.ToggleCanMove();
            //Reactivate Items
            _inventory.HideHandItem();
        }
    }
}
