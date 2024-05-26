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

    public GameObject AdjustInventorySlots
    {
        set
        {
            SwapObjectsInInventory(value);
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

    //Notes: Items not swapping visibility when pressing 1 and 2, Items not getting set to correct location when swapping.
}
