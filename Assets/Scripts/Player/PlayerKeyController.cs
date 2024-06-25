using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerKeyController : MonoBehaviour
{
    [SerializeField] private TMP_Text pauseMenuKeysCollectedText;
    public int keys;
    private KeyCubbyController _cubbyController;

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
