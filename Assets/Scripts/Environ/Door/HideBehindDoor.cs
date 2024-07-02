using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

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
    private PlayerInteractableController _interactableController;
    
    public bool isActive;
    private bool _isSwitching;

    private PostProcessVolume _postProcessing;

    [Header("Camera Rotation")] 
    private bool _shouldSwitch;
    private Quaternion _originalSpyCamRotation;

    private const float MaxIntensityValue = 60f;
    private const float MinXAngle = 0;
    private const float MaxXAngle = 5f;
    private const float MinYAngle = -100f;
    private const float MaxYAngle = -78f;
    
    private const float DistortionLerpSpeed = 5f;
    private const float MinCenterX = -.35f;
    private const float MaxCenterX = .35f;
    private const float MinCenterY = -.15f;
    private const float MaxCenterY = .15f;
    
    private float _currentCenterX = 0f;
    private float _currentCenterY = 0f;

    private GameObject _doorHud;
    private GameObject _inGameUI;
    
    private void Awake()
    {
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        _doorController = GetComponent<DoorController>();
        _player = GameObject.Find("Player");
        _FPC = FindObjectOfType<FirstPersonController>();
        _interactableController = FindObjectOfType<PlayerInteractableController>();
        
        hiddenLayer = LayerMask.NameToLayer("Default");
        defaultLayer = LayerMask.NameToLayer("Target");

        _inventory = FindAnyObjectByType<PlayerDualHandInventory>();

        _postProcessing = FindObjectOfType<PostProcessVolume>();
        
        _originalSpyCamRotation = _doorSpyHoleCamera.transform.rotation;

        _doorHud = GameObject.Find("DOORHUD");
        _inGameUI = GameObject.Find("INGAMEUI");
    }

    private void Start()
    {
        isActive = false;
        _isSwitching = false;

        _doorHud.SetActive(false);
    }

    private void Update()
    {
        if (isActive)
        {
            CheckForInput();
            HandleRotationInput();
        }
    }

    #region Change Hide State
    public void StartChangeState()
    {
        if (!_isSwitching)
        {
            StartCoroutine("ChangeHideState");
        }
    }

    private IEnumerator ChangeHideState()
    {
        isActive = !isActive;
        _isSwitching = true;
        if (isActive)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            _mainCamera.transform.GetChild(2).gameObject.SetActive(false);
            _FPC.ToggleCanMove();
            _doorController.Invoke("HandleDoor", 0);
            //_inventory.HideHandItem();
            yield return new WaitForSeconds(1f);
            ToggleDoorUI();
            _playerCam.Priority = 0;
            _doorHideCamera.Priority = 5;
            _doorController.Invoke("HandleDoor", 2f);
            Invoke("HidePlayer", 1.5f);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("InteractableObject");
            GetComponent<InteractableObject>().enabled = true;
            _mainCamera.transform.GetChild(2).gameObject.SetActive(true);
            _doorController.Invoke("HandleDoor", 0);
            //_inventory.HideHandItem();
            yield return new WaitForSeconds(1f);
            ToggleDoorUI();
            _playerCam.Priority = 5;
            _doorHideCamera.Priority = 0;
            _doorController.Invoke("HandleDoor", 2f);
            Invoke("HidePlayer", 1f);

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
        if (Input.GetMouseButtonDown(0) && _isSwitching == false)
        {
            //RaycastToMousePosition();
            _isSwitching = true;
            SwapDoorCameraPosition();
            Invoke("InteractDelay", 1f);

            StartCoroutine(_interactableController.InteractionSpamPrevention());
        }
        else if(Input.GetKeyDown(KeyCode.E) && _isSwitching == false)
        {
            StartChangeState();
        }
    }
    
    private void HandleRotationInput()
    {
        if (_doorSpyHoleCamera.Priority > _doorHideCamera.Priority)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (!_isSwitching)
            {
                _doorSpyHoleCamera.transform.Rotate(Vector3.up, mouseX, Space.World);
                _doorSpyHoleCamera.transform.Rotate(Vector3.right, -mouseY, Space.Self);

                
                Vector3 clampedRotation = _doorSpyHoleCamera.transform.localEulerAngles;
                clampedRotation.x = ClampSpyCamAngle(clampedRotation.x, MinXAngle, MaxXAngle); 
                clampedRotation.y = ClampSpyCamAngle(clampedRotation.y, MinYAngle, MaxYAngle); 
                clampedRotation.z = 0;

                _doorSpyHoleCamera.transform.localEulerAngles = clampedRotation;

                _postProcessing.profile.TryGetSettings(out LensDistortion lensDistortion);

                if (lensDistortion != null)
                {
                    _currentCenterX = Mathf.Clamp(_currentCenterX - (mouseX * Time.deltaTime * DistortionLerpSpeed), MinCenterX, MaxCenterX);
                    _currentCenterY = Mathf.Clamp(_currentCenterY - (mouseY * Time.deltaTime * DistortionLerpSpeed), MinCenterY, MaxCenterY);

                    lensDistortion.centerX.value = _currentCenterX;
                    lensDistortion.centerY.value = _currentCenterY;
                }
            }
        }
    }



    private float ClampSpyCamAngle(float angle, float min, float max)
    {
        if (angle < -180f) angle += 360f;
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
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

                StartCoroutine(_interactableController.InteractionSpamPrevention());
            }
        }
    }
    #endregion

    #region Peep Hole Camera
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
                StartCoroutine(HandleLensDistortionIntensity(lensDistortion, MaxIntensityValue, .75f, false));
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
            _doorSpyHoleCamera.transform.rotation = _originalSpyCamRotation;
            
            //return back to 0 distortion
            if (lensDistortion != null)
            {
                StartCoroutine(HandleLensDistortionIntensity(lensDistortion, 0f, .75f, true));
            }

            // Deactivate SpyHole Exterior Box Collider to prevent confusing collision outside of the door
            Collider[] colliders = _doorSpyHoleCamera.gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
    }


    private IEnumerator HandleLensDistortionIntensity(LensDistortion lD, float targetValue, float duration, bool resetCenter)
    {
        float startingDistortionIntensity = lD.intensity.value;
        float startingCenterX = lD.centerX.value;
        float startingCenterY = lD.centerY.value;
        float timer = 0f;

        while (timer < duration)
        {
            lD.intensity.value = Mathf.Lerp(startingDistortionIntensity, targetValue, timer / duration);

            if (resetCenter)
            {
                lD.centerX.value = Mathf.Lerp(startingCenterX, 0f, timer / duration);
                lD.centerY.value = Mathf.Lerp(startingCenterY, 0f, timer / duration);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        lD.intensity.value = targetValue;

        if (resetCenter)
        {
            lD.centerX.value = 0f;
            lD.centerY.value = 0f;
            _currentCenterX = 0f;
            _currentCenterY = 0f;
        }
    }

    private void ToggleDoorUI()
    {
        _doorHud.SetActive((!_doorHud.activeSelf));
        _inGameUI.SetActive(!_inGameUI.activeSelf);
    }
    #endregion

    private void HidePlayer()
    {
        if (isActive)
        {
            _player.layer = hiddenLayer;
            _inventory.HideHandItem();
        }
        else
        {
            _player.layer = defaultLayer;
            _FPC.ToggleCanMove();
            _inventory.ShowCurrentIndexItem();
        }
    }
}