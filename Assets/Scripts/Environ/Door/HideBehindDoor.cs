using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Rendering.PostProcessing;

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

    private PostProcessVolume _postProcessing;

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

        _postProcessing = FindObjectOfType<PostProcessVolume>();
        
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
    //old method
    // private void SwapDoorCameraPosition()
    // {
    //     if(_doorHideCamera.Priority > 0)
    //     {
    //         _doorHideCamera.Priority = 0;
    //         _doorSpyHoleCamera.Priority = 5;
    //         
    //         //Activate SpyHole Exterior Box Collider to allow the player to return from spy hole camera position
    //         Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
    //         foreach(Collider collider in colliders)
    //         {
    //             collider.enabled = !collider.enabled;
    //         }
    //     }
    //     else
    //     {
    //         _doorSpyHoleCamera.Priority = 0;
    //         _doorHideCamera.Priority = 5;
    //
    //         //Deactivate SpyHole Exterior Box Collider to prevent confusing collision outside of the door
    //         Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
    //         foreach (Collider collider in colliders)
    //         {
    //             collider.enabled = !collider.enabled;
    //         }
    //     }
    // }
    
    //new method to apply fish eye effect to the post processing volume
    private void SwapDoorCameraPosition()
    {
        _postProcessing.profile.TryGetSettings(out LensDistortion lensDistortion);
        
        if (_doorHideCamera.Priority > 0)
        {
            _doorHideCamera.Priority = 0;
            _doorSpyHoleCamera.Priority = 5;

            //move to 60 distortion intensity
            if (lensDistortion != null)
            {
                StartCoroutine(HandleLensDistortionIntensity(lensDistortion, 60f, .75f));
            }

            // Activate SpyHole Exterior Box Collider to allow the player to return from spy hole camera position
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
        else
        {
            _doorSpyHoleCamera.Priority = 0;
            _doorHideCamera.Priority = 5;

            //return back to 0 distortion
            if (lensDistortion != null)
            {
                StartCoroutine(HandleLensDistortionIntensity(lensDistortion, 0f, .75f));
            }

            // Deactivate SpyHole Exterior Box Collider to prevent confusing collision outside of the door
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
    }

    private IEnumerator HandleLensDistortionIntensity(LensDistortion lD, float targetValue, float duration)
    {
        float startingDistortionIntensity = lD.intensity.value;
        float timer = 0f;

        while (timer < duration)
        {
            lD.intensity.value = Mathf.Lerp(startingDistortionIntensity, targetValue, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        lD.intensity.value = targetValue;
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
