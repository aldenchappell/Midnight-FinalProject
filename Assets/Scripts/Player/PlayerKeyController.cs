using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerKeyController : MonoBehaviour
{
    [SerializeField] private TMP_Text pauseMenuKeysCollectedText;
    [SerializeField] private Image inGameKeysImage;
    [SerializeField] private TMP_Text inGameKeysCollectedText;
    public int keys;
    private KeyCubbyController _cubbyController;
    private SkullDialogue _skullDialogue;

    private void Awake()
    {
        _skullDialogue = FindObjectOfType<SkullDialogue>();
        
        FadeUI fadeUI = FindObjectOfType<FadeUI>();
        if (inGameKeysCollectedText != null && inGameKeysImage != null)
        {
            fadeUI.fadeDuration = 3.0f;
            fadeUI.FadeOutText(inGameKeysCollectedText);
            fadeUI.FadeOutImage(inGameKeysImage);
        }
    }

    private void Start()
    {
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
        
        FadeUI fadeUI = FindObjectOfType<FadeUI>();
        if (inGameKeysCollectedText != null && inGameKeysImage != null)
        {
            fadeUI.fadeDuration = 3.0f;
            fadeUI.FadeInAndOutText(inGameKeysCollectedText);
            fadeUI.FadeInAndOutImage(inGameKeysImage);
        }
        
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
        if (_skullDialogue != null && !SkullDialogueLineHolder.Instance.IsAudioSourcePlaying() && SceneManager.GetActiveScene().name != "LOBBY")
        {
            SkullDialogueLineHolder.Instance.PlaySpecificClip(
                SkullDialogueLineHolder.Instance.levelCompletedClips
                    [Random.Range(0, SkullDialogueLineHolder.Instance.levelCompletedClips.Length)]);
        }
    }
}