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
        //Debug.Log("Awake: Current Level Name: " + _levelName);

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
            case "FLOOR ONE":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorOneOpeningClip);
                break;
            case "FLOOR TWO":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorTwoOpeningClip);
                break;
            case "FLOOR THREE":
                PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorThreeOpeningClip);
                break;
            default:
                Debug.Log("Error playing level opening clip: SkullDialogue/PlayLevelOpeningClip");
                break;
        }

        LevelCompletionManager.Instance.SetSkullDialoguePlayed(levelName);
        LevelCompletionManager.Instance.SaveCurrentLevelAsLoaded(levelName);
        Debug.Log("PlayLevelOpeningClip: Marked dialogue as played and saved level as loaded for " + levelName);
    }

    public void TogglePickedUp()
    {
        pickedUp = !pickedUp;
        isSkullActive = pickedUp;
        Debug.Log("TogglePickedUp: pickedUp=" + pickedUp + ", isSkullActive=" + isSkullActive);

        if (pickedUp)
        {
            if (!_hasBeenPickedUp)
            {
                _hasBeenPickedUp = true;
                SkullDialogueLineHolder.Instance.audioSource.transform.position = transform.position;
                SkullDialogueLineHolder.Instance.audioSource.transform.SetParent(gameObject.transform);
                _interactableObject.onInteraction.RemoveListener(() => PlayLevelOpeningClip(_levelName));
                 Debug.Log("TogglePickedUp: First time picked up, removed listener and set audio source position.");
            }

            if (_dialogueCoroutine != null)
            {
                StopCoroutine(_dialogueCoroutine);
                Debug.Log("TogglePickedUp: Stopped previous dialogue coroutine.");
            }
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
        Debug.Log("PlaySpecificSkullDialogueClip: Playing clip " + clip.name);
        source.PlayOneShot(clip);
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[randomIndex]);
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        if (value)
        {
            PlaySpecificSkullDialogueClip(source, clip);
        }
    }
}