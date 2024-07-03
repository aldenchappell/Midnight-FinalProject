using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerKeyController : MonoBehaviour
{
    [SerializeField] private TMP_Text pauseMenuKeysCollectedText;
    [SerializeField] private TMP_Text inGameKeysCollectedText;
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

        // Play level completion clip if a key is collected
        PlayLevelCompletionClip();
    }

    public void PlaceKeyInCubby(int keyIndex)
    {
        if (_cubbyController != null && _cubbyController.IsSlotAvailable(keyIndex))
        {
            _cubbyController.PlaceKey(keyIndex);
            keys--;
            UpdateKeyUI();
            LevelCompletionManager.Instance.SetCollectedKeys(keys);

            // Play level completion clip when placing the last key in the cubby
            if (keys == 0)
            {
                PlayLevelCompletionClip();
            }
        }
        else
        {
            Debug.Log("Cannot place key in cubby. Slot not available or KeyCubbyController not found.");
        }
    }

    private void UpdateKeyUI()
    {
        pauseMenuKeysCollectedText.text = "Keys collected: " + keys;
        inGameKeysCollectedText.text = keys.ToString();
    }

    private void PlayLevelCompletionClip()
    {
        if (_skullDialogue != null && !SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            _skullDialogue.PlaySpecificSkullDialogueClip(SkullDialogueLineHolder.Instance.audioSource,
                SkullDialogueLineHolder.Instance.levelCompletedClips[Random.Range(0, SkullDialogueLineHolder.Instance.levelCompletedClips.Length)]);
        }
    }
}