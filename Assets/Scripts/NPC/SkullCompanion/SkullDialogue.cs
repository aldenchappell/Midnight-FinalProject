using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SkullDialogue : MonoBehaviour
{
    //Animation stuff
    public Animator skullAnimator;

    private Coroutine _wittyAssRemarksCoroutine;

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
        CheckSkullStatus();
    }

    private void CheckSkullStatus()
    {
        bool isSkullActiveInInventory = IsSkullActiveInInventory();
        
        //if constantine has been picked up at least once and is not active in inventory, start playing witty remarks
        if (_hasBeenPickedUp && !isSkullActiveInInventory)
        {
            //Debug.Log("Starting Witty Ass Remarks");
            if (_wittyAssRemarksCoroutine == null)
            {
                _wittyAssRemarksCoroutine = StartCoroutine(PlayWittyAssRemarks());
            }
        }
        else
        {
            //Debug.Log("Stopping Witty Ass Remarks");
            if (_wittyAssRemarksCoroutine != null)
            {
                StopCoroutine(_wittyAssRemarksCoroutine);
                _wittyAssRemarksCoroutine = null;
            }
        }
    }

    public bool IsSkullActiveInInventory()
    {
        for (int i = 0; i < _playerInventory.GetInventory.Length; i++)
        {
            if (_playerInventory.GetInventory[i] == gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public void TogglePickedUp()
    {
        _hasBeenPickedUp = true;
        
        SkullDialogueLineHolder.Instance.audioSource.transform.SetParent(gameObject.transform);
        SkullDialogueLineHolder.Instance.audioSource.transform.position = transform.position;
        _interactableObject.onInteraction.RemoveListener(() => PlayLevelOpeningClip(_levelName));
    }

    private IEnumerator PlayWittyAssRemarks()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkullDialogueLineHolder.Instance.GetRandomWaitTime());
            SkullDialogueLineHolder.Instance.PlayRandomClip(SkullDialogueLineHolder.Instance.wittyAssRemarks);
        }
    }

    private void PlayLevelOpeningClip(string levelName)
    {
        if (LevelCompletionManager.Instance.HasSkullDialogueBeenPlayed(levelName) || LevelCompletionManager.Instance.IsLevelCompleted(levelName))
        {
            return;
        }

        switch (levelName)
        {
            case "LOBBY":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying() && !LevelCompletionManager.Instance.hasCompletedLobby)
                    SkullDialogueLineHolder.Instance.PlaySpecificClip(
                        SkullDialogueLineHolder.Instance.lobbyOpeningClip);
                break;
            case "FLOOR ONE":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                    SkullDialogueLineHolder.Instance.PlaySpecificClip(
                        SkullDialogueLineHolder.Instance.floorOneOpeningClip);
                break;
            case "FLOOR TWO":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                    SkullDialogueLineHolder.Instance.PlaySpecificClip(
                        SkullDialogueLineHolder.Instance.floorTwoOpeningClip);
                break;
            case "FLOOR THREE":
                if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                    SkullDialogueLineHolder.Instance.PlaySpecificClip(
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
}