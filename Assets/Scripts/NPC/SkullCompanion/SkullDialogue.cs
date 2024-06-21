using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class SkullDialogue : MonoBehaviour
{
    // [SerializeField] private List<AudioClip> pickedUpAudioClips;
    // [SerializeField] private List<string> pickedUpMessages;
    // [SerializeField] private List<AudioClip> droppedAudioClips;
    // [SerializeField] private List<string> droppedMessages;
    public bool isFirstClip;
    
    
    public bool pickedUp;
    private Coroutine _dialogueCoroutine;

    // [SerializeField] private float minDelayBetweenMessages = 5f;
    // [SerializeField] private float maxDelayBetweenMessages = 12.5f;
    //
    // [SerializeField] private GameObject dialogueUI;
    // [SerializeField] private TMP_Text dialogueText;
    // [SerializeField] private float textPrintingSpeed = 0.05f;

    public bool isSkullActive;

    private PlayerDualHandInventory _playerInventory; // Reference to the player's inventory

    private void Awake()
    {
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>(); // Find the player's inventory
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateSkullActiveStatus(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateSkullActiveStatus(1);
        }
    }

    public void TogglePickedUp()
    {
        pickedUp = !pickedUp;
        isSkullActive = pickedUp;

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
        }

        // if (pickedUp)
        // {
        //     _dialogueCoroutine = StartCoroutine(DialogueSequence(pickedUpAudioClips, pickedUpMessages));
        // }
        // else
        // {
        //     dialogueUI.SetActive(false);
        //     _dialogueCoroutine = StartCoroutine(DialogueSequence(droppedAudioClips, droppedMessages));
        // }
    }

    private void UpdateSkullActiveStatus(int slotIndex)
    {
        if (_playerInventory != null && _playerInventory.GetInventory.Length > slotIndex)
        {
            isSkullActive = _playerInventory.GetInventory[slotIndex] == gameObject;
            pickedUp = isSkullActive;
        }
    }

    // private IEnumerator DialogueSequence(List<AudioClip> audioClips, List<string> messages)
    // {
    //     if (audioClips.Count == 0 || messages.Count == 0)
    //     {
    //         yield break;
    //     }
    //     
    //     if (audioClips.Count != messages.Count)
    //     {
    //         Debug.LogWarning("Audio clips and messages lists must be of the same length.");
    //         yield break;
    //     }
    //
    //     for (int i = 0; i < messages.Count; i++)
    //     {
    //         dialogueUI.SetActive(true);
    //
    //         dialogueText.text = "";
    //
    //         string currentMessage = messages[i];
    //
    //         _audioSource.clip = audioClips[i];
    //         _audioSource.Play();
    //         
    //         for (int j = 0; j < currentMessage.Length; j++)
    //         {
    //             dialogueText.text += currentMessage[j];
    //             yield return new WaitForSeconds(textPrintingSpeed);
    //         }
    //         
    //         yield return new WaitForSeconds(_audioSource.clip.length);
    //         
    //         dialogueUI.SetActive(false);
    //         
    //         float delay = Random.Range(minDelayBetweenMessages, maxDelayBetweenMessages);
    //         yield return new WaitForSeconds(delay);
    //     }
    // }
}
