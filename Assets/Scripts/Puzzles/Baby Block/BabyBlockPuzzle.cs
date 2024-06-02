using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class BabyBlockPuzzle : MonoBehaviour
{
    [Header("Puzzle UI")]
    [SerializeField] GameObject puzzleUI;
    [Header("Animation Child")]
    [SerializeField] GameObject animationChild;

    private PlayerDualHandInventory  _playerInv;
    private FirstPersonController _FPC;
    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _puzzleCam;
    private Camera _mainCam;

    private bool _isActive;

    private void Awake()
    {
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("BabyBlockPuzzleCam").GetComponent<CinemachineVirtualCamera>();
        _playerInv = GameObject.FindFirstObjectByType<PlayerDualHandInventory>();
        _mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
        _FPC = GameObject.FindFirstObjectByType<FirstPersonController>();
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
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        if(puzzleUI.activeSelf)
        {
            _FPC.ToggleCanMove();
            _playerCam.Priority = 0;
            _puzzleCam.Priority = 10;

            List<Transform> children = new List<Transform>();
            children.AddRange(animationChild.GetComponentsInChildren<Transform>());
            children.Remove(animationChild.transform);

            foreach (Transform child in children)
            {
                if (child.GetComponent<Collider>() != null)
                {
                    child.GetComponent<Collider>().enabled = true;
                }
            }

            _isActive = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _FPC.ToggleCanMove();
            _playerCam.Priority = 10;
            _puzzleCam.Priority = 0;

            List<Transform> children = new List<Transform>();
            children.AddRange(animationChild.GetComponentsInChildren<Transform>());
            children.Remove(animationChild.transform);

            foreach (Transform child in children)
            {
                if(child.GetComponent<Collider>() != null)
                {
                    child.GetComponent<Collider>().enabled = false;
                }
            }

            _isActive = false;
        }
    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateObject(false, 90);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateObject(false, -90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RotateObject(true, -90);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateObject(true, 90);
        }
        if(Input.GetMouseButtonDown(0))
        {
            RaycastToMousePosition();
        }
    }

    private void RotateObject(bool axis, int rotation)
    {
        if (axis)
        {
            animationChild.transform.Rotate(rotation, 0, 0);
        }
        else
        {
            animationChild.transform.Rotate(0, rotation, 0);
        }
    }
    
    private void RaycastToMousePosition()
    {
        RaycastHit hit;
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if(!objectHit.CompareTag("Untagged"))
            {
                CheckForCorrectObject(objectHit);
            }
        }
    }

    private void CheckForCorrectObject(Transform slot)
    {
        GameObject rightItem = null;
        GameObject[] currentItems = _playerInv.GetInventory;
        foreach(GameObject item in currentItems)
        {
            if(item != null)
            {
                if (item.transform.CompareTag(slot.tag))
                {
                    rightItem = item;
                }
            }
        }
        if(rightItem == null)
        {
            print("Failed");
        }
        else
        {
            _playerInv.PlaceObjectInPuzzle(slot.gameObject, System.Array.IndexOf(currentItems, rightItem), animationChild);
        }
    }
    
}
