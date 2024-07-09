using System;
using UnityEngine;

public class CubbyKey : MonoBehaviour
{
    public SO_CubbyKey cubbyKey;
    public GameObject keyObject;
    public GameObject particles;
    private void Start()
    {
        if (cubbyKey.placed)
        {
            keyObject.SetActive(true);
            particles.SetActive(false);
            Debug.Log("Activating cubby key. Has been placed.");
        }
        else
        {
            keyObject.SetActive(false);
            particles.SetActive(true);
            Debug.Log("Deactivating cubby key. Not placed.");
        }
    }
}
