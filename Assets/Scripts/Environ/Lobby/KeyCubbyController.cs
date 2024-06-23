using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCubbyController : MonoBehaviour
{
    public List<GameObject> keySlots = new List<GameObject>(); 

    private void Start()
    {
        InitializeKeySlots();
    }

    private void InitializeKeySlots()
    {
        //loop through the key slots and activate/deactivate based on placed keys
        for (int i = 0; i < keySlots.Count; i++)
        {
            bool keyPlaced = IsKeyPlaced(i);
            keySlots[i].SetActive(keyPlaced);
        }
    }

    public void PlaceKey(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keySlots.Count)
        {
            keySlots[keyIndex].SetActive(false); //deactivate the slot where the key is placed
        }
        else
        {
            Debug.Log("Invalid key index for placing key in cubby.");
        }
    }

    public bool IsSlotAvailable(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keySlots.Count)
        {
            return keySlots[keyIndex].activeSelf; //return true if the slot is available 
        }
        else
        {
            Debug.LogWarning("Invalid key index for checking slot availability in cubby.");
            return false;
        }
    }

    public bool IsKeyPlaced(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keySlots.Count)
        {
            return !keySlots[keyIndex].activeSelf; //return true if the slot is not active (key placed)
        }
        else
        {
            Debug.LogWarning("Invalid key index for checking placed key in cubby.");
            return false;
        }
    }

    public void ResetCubby()
    {
        foreach (var slot in keySlots)
        {
            slot.SetActive(true);
        }
    }
}