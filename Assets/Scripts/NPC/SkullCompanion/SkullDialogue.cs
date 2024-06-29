using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkullDialogue : MonoBehaviour, IPlaySkullDialogue
{
    public bool pickedUp;
    private Coroutine _dialogueCoroutine;
    private Coroutine _hintCoroutine; 
    public bool isSkullActive;

    private PlayerDualHandInventory _playerInventory; 
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
        if (LevelCompletionManager.Instance.HasSkullDialogueBeenPlayed(levelName) || LevelCompletionManager.Instance.IsLevelCompleted(levelName))
        {
            return; 
        }

        switch (levelName)
        {
            case "LOBBY":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying() && !LevelCompletionManager.Instance.hasCompletedLobby)
                 PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.lobbyOpeningClip);
                break;
            case "FLOOR ONE":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                    PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorOneOpeningClip);
                break;
            case "FLOOR TWO":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                    PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorTwoOpeningClip);
                break;
            case "FLOOR THREE":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                    PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.floorThreeOpeningClip);
                break;
            default:
                Debug.Log("Error playing level opening clip: SkullDialogue/PlayLevelOpeningClip");
                break;
        }

        LevelCompletionManager.Instance.SetSkullDialoguePlayed(levelName);
        LevelCompletionManager.Instance.SaveCurrentLevelAsLoaded(levelName);
        LevelCompletionManager.Instance.StartLevel(levelName, GetPuzzlesForLevel(levelName));
    }

    private List<SO_Puzzle> GetPuzzlesForLevel(string levelName)
    {
        switch (levelName)
        {
            case "LOBBY":
                return LevelCompletionManager.Instance.lobbyPuzzles;
            case "FLOOR ONE":
                return LevelCompletionManager.Instance.level1Puzzles;
            case "FLOOR TWO":
                return LevelCompletionManager.Instance.level2Puzzles;
            case "FLOOR THREE":
                return LevelCompletionManager.Instance.level3Puzzles;
            default:
                return new List<SO_Puzzle>();
        }
    }


    public void TogglePickedUp()
    {
        pickedUp = !pickedUp;
        isSkullActive = pickedUp;

        if (pickedUp)
        {
            if (!_hasBeenPickedUp)
            {
                _hasBeenPickedUp = true;
                SkullDialogueLineHolder.Instance.audioSource.transform.position = transform.position;
                SkullDialogueLineHolder.Instance.audioSource.transform.SetParent(gameObject.transform);
                _interactableObject.onInteraction.RemoveListener(() => PlayLevelOpeningClip(_levelName));
            }

            if (_dialogueCoroutine != null)
            {
                StopCoroutine(_dialogueCoroutine);
            }

            if (_hintCoroutine != null)
            {
                StopCoroutine(_hintCoroutine);
            }

            _hintCoroutine = StartCoroutine(HintCoroutine());
        }
        else
        {
            if (_hintCoroutine != null)
            {
                StopCoroutine(_hintCoroutine);
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

    private IEnumerator HintCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkullDialogueLineHolder.Instance.GetRandomWaitTIme()); 
            PlayHintBasedOnRemainingPuzzles();
        }
    }

    private void PlayHintBasedOnRemainingPuzzles()
    {
        List<string> remainingPuzzles = LevelCompletionManager.Instance.currentLevelPuzzles;

        AudioClip hintClip = SkullDialogueLineHolder.Instance.GetHintClipForRemainingPuzzles(remainingPuzzles);
        if (hintClip != null && !SkullDialogueLineHolder.Instance.audioSource.isPlaying)
        {
            PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource, hintClip);
        }
    }

    #region Dialogue Interface Methods

    public void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
            source.PlayOneShot(clip);
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            if (clips.Length == 0) return;
            int randomIndex = Random.Range(0, clips.Length);
            source.PlayOneShot(clips[randomIndex]);
        }
        
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            if (value)
            {
                PlaySpecificSkullDialogueClip(source, clip);
            }
        }
    }

    public IEnumerator PlaySkullDialoguePuzzleHintClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip)
    {
        yield return null;
    }

    #endregion
}