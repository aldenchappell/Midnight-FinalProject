using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class FuseBox : MonoBehaviour
{
    private Animator _animator;

    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _puzzleCam;
    private Camera _mainCam;
    private Collider _boxCollider;
    private PlayerDualHandInventory _inventory;
    private FirstPersonController _FPC;

    private bool _isActive;
    private bool _fuseIn;

    private void Awake()
    {
        _inventory = GameObject.FindAnyObjectByType<PlayerDualHandInventory>();
        _FPC = GameObject.FindFirstObjectByType<FirstPersonController>();
        _mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
        _animator = transform.GetComponent<Animator>();
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();
        _boxCollider = transform.GetComponent<Collider>();
        _isActive = false;
    }

    private void Update()
    {
        if(_isActive)
        {
            CheckForInput();
        }
    }

    public void ActivatePuzzle()
    {
        _isActive = !_isActive;
        _boxCollider.enabled = !_boxCollider.enabled;
        if (_isActive)
        {
            _playerCam.Priority = 0;
            _puzzleCam.Priority = 10;
            _FPC.ToggleCanMove();
            AnimationsTrigger("Open");
            FindObjectOfType<GlobalCursorManager>().EnableCursor();
        }
        else
        {
            _playerCam.Priority = 10;
            _puzzleCam.Priority = 0;
            _FPC.ToggleCanMove();
            FindObjectOfType<GlobalCursorManager>().DisableCursor();
        }
    }

    private void CheckForInput()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ActivatePuzzle();
        }
        else if(Input.GetMouseButtonDown(0))
        {
            RaycastToMousePosition();
        }
    }

    private void RaycastToMousePosition()
    {
        RaycastHit hit;
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Fuse"))
            {
                PlaceFuse(objectHit.gameObject);
            }
            if (objectHit.transform.CompareTag("Lever") && _fuseIn)
            {
                AnimationsTrigger("PowerOn");
                ActivatePuzzle();
            }
        }
    }

    private void PlaceFuse(GameObject fuseShadow)
    {
        GameObject[] currentItems = _inventory.GetInventory;
        foreach (GameObject item in currentItems)
        {
            if (item != null)
            {
                if (item.transform.CompareTag(fuseShadow.tag))
                {
                    AnimationsTrigger("Place");
                    _inventory.RemoveObject = item;
                    _fuseIn = true;
                }
            }
        }
    }

    private void AnimationsTrigger(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
