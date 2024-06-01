using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BabyBlockPuzzle : MonoBehaviour
{
    [Header("Puzzle UI")]
    [SerializeField] GameObject puzzleUI;
    [Header("Animation Child")]
    [SerializeField] GameObject animationChild;

    private PlayerDualHandInventory  _playerInv;
    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _puzzleCam;
    private Camera _mainCam;

    private void Awake()
    {
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("BabyBlockPuzzleCam").GetComponent<CinemachineVirtualCamera>();
        _playerInv = GameObject.FindFirstObjectByType<PlayerDualHandInventory>();
        _mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        CheckForInput();
    }

    public void ActivatePuzzle()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        if(puzzleUI.activeSelf)
        {
            _playerCam.Priority = 0;
            _puzzleCam.Priority = 10;
            Transform[] children = transform.GetChild(2).GetComponentsInChildren<Transform>();
            foreach(Transform child in children)
            {
                child.GetComponent<BoxCollider>().enabled = true;
            }
        }
        else
        {
            _playerCam.Priority = 10;
            _puzzleCam.Priority = 0;
            Transform[] children = transform.GetChild(2).GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                child.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    //Add left click functionality
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
            if(!objectHit.CompareTag("Default"))
            {
                PlaceObjectInSlot(objectHit);
            }
        }
    }

    private void PlaceObjectInSlot(Transform obj)
    {
        GameObject rightItem = null;
        GameObject[] currentItems = _playerInv.GetInventory;
        foreach(GameObject item in currentItems)
        {
            if(item.transform.CompareTag(obj.tag))
            {
                rightItem = item;
            }
        }
        if(rightItem = null)
        {
            print("Failed");
        }
        else
        {
            print("Succeeded");
        }
    }
}
