using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDualHandInventory : MonoBehaviour
{
    [Header("Item Hand Location")]
    [SerializeField] Transform handPosition;

    private GameObject[] _inventorySlots;
    private int _currentIndexSelected;

    public GameObject[] GetInventory
    {
        get
        {
            return _inventorySlots;
        }
    }

    private void Start()
    {
        _inventorySlots = new GameObject[2];
        _currentIndexSelected = 0;
    }

    //Adjsut inventory when picking up new interactable object
    public GameObject AdjustInventorySlots
    {
        set
        {
            SwapObjectsInInventory(value);
        }
    }
    //Place interactable object in inventory onto shadow position and adjust inventory
    public GameObject PlaceObject
    {
        set
        {
            PlaceObjectFromInventory(value);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentIndexSelected = 0;
            ShowCurrentIndexItem();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentIndexSelected = 1;
            ShowCurrentIndexItem();
        }
        
    }

    //Swap current interactable object in hand for another in environment
    private void SwapObjectsInInventory(GameObject newObject)
    {
        if (_inventorySlots[_currentIndexSelected] != null)
        {
            _inventorySlots[_currentIndexSelected].SetActive(true);
            _inventorySlots[_currentIndexSelected].transform.position = newObject.transform.position;
            _inventorySlots[_currentIndexSelected].transform.parent = null;
        }
        newObject.transform.parent = this.gameObject.transform;
        _inventorySlots[_currentIndexSelected] = newObject;
        ShowCurrentIndexItem();
    }

    //Place interactable object in hand in shadow position
    private void PlaceObjectFromInventory(GameObject objectPosition)
    {
        if (_inventorySlots[_currentIndexSelected] != null)
        {
            if(_inventorySlots[_currentIndexSelected].transform.tag == objectPosition.transform.tag)
            {
                if(objectPosition.transform.parent != null)
                {
                    _inventorySlots[_currentIndexSelected].transform.parent = objectPosition.transform.parent;
                }
                else
                {
                    _inventorySlots[_currentIndexSelected].transform.parent = null; 
                }
                
                _inventorySlots[_currentIndexSelected].transform.position = objectPosition.transform.position;
                _inventorySlots[_currentIndexSelected].transform.eulerAngles = objectPosition.transform.eulerAngles;
                Destroy(_inventorySlots[_currentIndexSelected].GetComponent<InteractableObject>());
                Destroy(objectPosition);
                _inventorySlots[_currentIndexSelected] = null;
            }
        }
    }

    //Special place functionality for easier use in puzzles
    public void PlaceObjectInPuzzle(GameObject objectPosition, int index, GameObject parentObject)
    {
        if (_inventorySlots[index] != null)
        {
            if (_inventorySlots[index].transform.tag == objectPosition.transform.tag)
            {
                if (parentObject != null)
                {
                    _inventorySlots[index].transform.parent = parentObject.transform;
                }
                else
                {
                    _inventorySlots[index].transform.parent = null;
                }
                _inventorySlots[index].transform.position = objectPosition.transform.position;
                _inventorySlots[index].transform.eulerAngles = objectPosition.transform.eulerAngles;
                Destroy(_inventorySlots[index].GetComponent<InteractableObject>());
                Destroy(_inventorySlots[index].GetComponent<Collider>());
                Destroy(objectPosition);
                _inventorySlots[index].SetActive(true);
                _inventorySlots[index] = null;
                
            }
        }
    }

    //Show the interactable object in the current inventory index selected by player
    private void ShowCurrentIndexItem()
    {
        foreach(GameObject obj in _inventorySlots)
        {
            if(obj != null)
            {
                obj.SetActive(false);
            }
        }
        if (_inventorySlots[_currentIndexSelected] != null)
        {
            _inventorySlots[_currentIndexSelected].SetActive(true);
            _inventorySlots[_currentIndexSelected].transform.parent = this.gameObject.transform;
            _inventorySlots[_currentIndexSelected].transform.localPosition = handPosition.localPosition;
        }
    }


}
