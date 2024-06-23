using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private TMP_Text pauseMenuKeysCollectedText;
    public int keys;

    private KeyCubbyController cubbyController;

    private void Start()
    {
        keys = 0;
        UpdateKeyUI();
        
        cubbyController = FindObjectOfType<KeyCubbyController>(); // Find the KeyCubbyController in the scene
    }

    public void CollectKey()
    {
        keys++;
        UpdateKeyUI();
        LevelCompletionManager.Instance.CollectKey();

        //place collected key in the cubby if cubby is available
        if (cubbyController != null)
        {
            PlaceKeyInCubby(keys - 1); // place key in cubby at the corresponding index
        }
    }

    private void UpdateKeyUI()
    {
        pauseMenuKeysCollectedText.text = "Keys collected: " + keys;
    }

    private void PlaceKeyInCubby(int keyIndex)
    {
        if (cubbyController.IsSlotAvailable(keyIndex))
        {
            cubbyController.PlaceKey(keyIndex); //place key in the cubby
        }
        else
        {
            Debug.Log("Cannot place key in cubby slot " + keyIndex + ". Slot already filled.");
            // Optionally, handle UI/feedback for inability to place key
        }
    }
    
    public void ResetLobby()
    {
        keys = 0;
        UpdateKeyUI();
        cubbyController.ResetCubby(); //reset the key cubby 
    }
}
