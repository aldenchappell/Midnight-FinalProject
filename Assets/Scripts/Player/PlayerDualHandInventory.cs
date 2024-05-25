using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDualHandInventory : MonoBehaviour
{
    [Header("Storage Location for Inventory")]
    [SerializeField] Vector3 storageLocation;

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
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentIndexSelected = 1;
        }
    }

    private void SwapObjectsInInventory(GameObject newObject)
    {
        if (_inventorySlots[_currentIndexSelected] != null)
        {
            _inventorySlots[_currentIndexSelected].SetActive(true);
            _inventorySlots[_currentIndexSelected].transform.position = newObject.transform.position;
        }
        newObject.transform.position = storageLocation;
        newObject.SetActive(false);
        _inventorySlots[_currentIndexSelected] = newObject; 
    }
}
