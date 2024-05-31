using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDualHandInventory : MonoBehaviour
{
    [Header("Item Hand Location")]
    [SerializeField] Transform handPosition;

    private GameObject[] _inventorySlots;
    private int _currentIndexSelected;

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
                _inventorySlots[_currentIndexSelected].transform.parent = null;
                _inventorySlots[_currentIndexSelected].transform.position = objectPosition.transform.position;
                _inventorySlots[_currentIndexSelected].transform.eulerAngles = objectPosition.transform.eulerAngles;
                Destroy(_inventorySlots[_currentIndexSelected].GetComponent<InteractableObject>());
                Destroy(objectPosition);
                _inventorySlots[_currentIndexSelected] = null;
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
