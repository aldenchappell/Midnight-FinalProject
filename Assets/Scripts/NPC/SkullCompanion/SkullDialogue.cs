using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkullDialogue : MonoBehaviour
{
    [SerializeField] private List<AudioClip> pickedUpAudioClips;
    [SerializeField] private List<string> pickedUpMessages;
    [SerializeField] private List<AudioClip> droppedAudioClips;
    [SerializeField] private List<string> droppedMessages;
    private AudioSource _audioSource;

    public bool pickedUp;
    private Coroutine _dialogueCoroutine;

    [SerializeField] private float minDelayBetweenMessages = 5f;
    [SerializeField] private float maxDelayBetweenMessages = 12.5f;

    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float textPrintingSpeed = 0.05f;

    public bool isSkullActive;

    private PlayerDualHandInventory _playerInventory; // Reference to the player's inventory

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>(); // Find the player's inventory
    }

    private void Update()
    {
        // Check for key presses to switch inventory slots
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

        if (pickedUp)
        {
            _dialogueCoroutine = StartCoroutine(DialogueSequence(pickedUpAudioClips, pickedUpMessages));
        }
        else
        {
            dialogueUI.SetActive(false);
            _dialogueCoroutine = StartCoroutine(DialogueSequence(droppedAudioClips, droppedMessages));
        }
    }

    public bool IsSkullActiveInInventory()
    {
        return _playerInventory != null && _playerInventory.GetInventory[0] == gameObject;
    }

    private void UpdateSkullActiveStatus(int slotIndex)
    {
        if (_playerInventory != null && _playerInventory.GetInventory.Length > slotIndex)
        {
            isSkullActive = _playerInventory.GetInventory[slotIndex] == gameObject;
            pickedUp = isSkullActive;
        }
    }

    private IEnumerator DialogueSequence(List<AudioClip> audioClips, List<string> messages)
    {
        if (audioClips.Count == 0 || messages.Count == 0)
        {
            yield break;
        }

        // Ensure audio clips and messages lists are of the same length
        if (audioClips.Count != messages.Count)
        {
            Debug.LogWarning("Audio clips and messages lists must be of the same length.");
            yield break;
        }

        for (int i = 0; i < messages.Count; i++)
        {
            // Enable the dialogue UI
            dialogueUI.SetActive(true);

            // Clear the dialogue text
            dialogueText.text = "";

            // Get the current message
            string currentMessage = messages[i];

            // Play the selected audio clip
            _audioSource.clip = audioClips[i];
            _audioSource.Play();

            // Type out the message progressively
            for (int j = 0; j < currentMessage.Length; j++)
            {
                dialogueText.text += currentMessage[j];
                yield return new WaitForSeconds(textPrintingSpeed);
            }

            // Wait for the audio clip to finish playing
            yield return new WaitForSeconds(_audioSource.clip.length);

            // Disable the dialogue UI after the message and audio clip are done
            dialogueUI.SetActive(false);

            // Wait for a random amount of time before playing the next clip
            float delay = Random.Range(minDelayBetweenMessages, maxDelayBetweenMessages);
            yield return new WaitForSeconds(delay);
        }
    }
}
