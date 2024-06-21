using UnityEngine;
using UnityEngine.SceneManagement;

public class SkullDialogue : MonoBehaviour, IPlaySkullDialogue
{
    

    public bool pickedUp;
    private Coroutine _dialogueCoroutine;
    public bool isSkullActive;

    private PlayerDualHandInventory _playerInventory; // Reference to the player's inventory
    private string _levelName;
    private InteractableObject _interactableObject;
    private bool _hasBeenPickedUp = false;

    private void Awake()
    {
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>();
        _levelName = SceneManager.GetActiveScene().name;

        _interactableObject = GetComponent<InteractableObject>();
        _interactableObject.onInteraction.AddListener(() => PlayLevelOpeningClip(_levelName));
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

    public void PlayLevelOpeningClip(string levelName)
    {
        if (LevelCompletionManager.Instance.HasSkullDialogueBeenPlayed(levelName))
        {
            return; 
        }

        switch (levelName)
        {
            case "LOBBY":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.lobbyOpeningClip);
                break;
            case "FLOORONE":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorOneOpeningClip);
                break;
            case "FLOORTWO":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorTwoOpeningClip);
                break;
            case "FLOORTHREE":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorThreeOpeningClip);
                break;
            default:
                Debug.Log("Error playing level opening clip: SkullDialogue/PlayLevelOpeningClip");
                break;
        }

        LevelCompletionManager.Instance.SetSkullDialoguePlayed(levelName);
        LevelCompletionManager.Instance.SaveCurrentLevelAsLoaded(levelName);
    }

    public void TogglePickedUp()
    {
        pickedUp = !pickedUp;
        isSkullActive = pickedUp;

        _hasBeenPickedUp = true;
        if (_hasBeenPickedUp)
        {
            _interactableObject.onInteraction.RemoveListener(() => PlayLevelOpeningClip(_levelName));
        }

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
        }
    }

    private void UpdateSkullActiveStatus(int slotIndex)
    {
        if (_playerInventory != null && _playerInventory.GetInventory.Length > slotIndex)
        {
            isSkullActive = _playerInventory.GetInventory[slotIndex] == gameObject;
            pickedUp = isSkullActive;
        }
    }

    public void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip)
    {
        SkullDialogueLineHolder.Instance.audioSource.transform.position = transform.position;
        source.PlayOneShot(clip);
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clip)
    {
        
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        
    }
}
