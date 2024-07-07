using System;
using UnityEngine;

public class CubbyKey : MonoBehaviour
{
    public SO_CubbyKey cubbyKey;
    public GameObject keyObject;
    
    private void Start()
    {
        if (cubbyKey.placed)
        {
            keyObject.SetActive(true);
            Debug.Log("Activating cubby key. Has been placed.");
        }
        else
        {
            keyObject.SetActive(false);
            Debug.Log("Deactivating cubby key. Not placed.");
        }
    }
}
