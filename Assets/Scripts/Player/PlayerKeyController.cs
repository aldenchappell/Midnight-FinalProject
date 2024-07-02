using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerKeyController : MonoBehaviour
{
    [SerializeField] private TMP_Text pauseMenuKeysCollectedText;
    public int keys;
    private KeyCubbyController _cubbyController;
    private SkullDialogue _skullDialogue;


    private void Awake()
    {
        _skullDialogue = FindObjectOfType<SkullDialogue>();
        
    }

    private void Start()
    {
        // Get the key count from LevelCompletionManager
        keys = LevelCompletionManager.Instance.GetCollectedKeys();
        UpdateKeyUI();

        
        
        if (SceneManager.GetActiveScene().name == "LOBBY")
        {
            _cubbyController = FindObjectOfType<KeyCubbyController>();
        }
    }

    public void CollectKey()
    {
        keys++;
        UpdateKeyUI();
        LevelCompletionManager.Instance.CollectKey();
        
        //Tell the player to return to the elevator to return the key
        if (_skullDialogue != null)
        {
            if(!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
                _skullDialogue.PlayRandomSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                SkullDialogueLineHolder.Instance.levelCompletedClips);
            else
            {
                //wait until current audio clip is done playing, then play a clip
                StartCoroutine(_skullDialogue.WaitForCurrentClipToEndAndPlay(
                    true,
                    SkullDialogueLineHolder.Instance.levelCompletedClips,
                    null));
                Debug.Log(SkullDialogueLineHolder.Instance.audioSource.clip);
            }
        }
    }

    public void PlaceKeyInCubby(int keyIndex)
    {
        if (_cubbyController != null && _cubbyController.IsSlotAvailable(keyIndex))
        {
            _cubbyController.PlaceKey(keyIndex);
            keys--; 
            UpdateKeyUI();
            LevelCompletionManager.Instance.SetCollectedKeys(keys);
        }
        else
        {
            Debug.Log("Cannot place key in cubby. Slot not available or KeyCubbyController not found.");
        }
    }

    private void UpdateKeyUI()
    {
        pauseMenuKeysCollectedText.text = "Keys collected: " + keys;
    }
}
