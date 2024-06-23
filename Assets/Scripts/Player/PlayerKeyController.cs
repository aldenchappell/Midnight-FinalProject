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
        keys = 0;
        UpdateKeyUI();

        if(SceneManager.GetActiveScene().name == "LOBBY")
            _cubbyController = FindObjectOfType<KeyCubbyController>();
    }

    public void CollectKey()
    {
        keys++;
        UpdateKeyUI();
        LevelCompletionManager.Instance.CollectKey();
        
        if (_cubbyController != null)
        {
            PlaceKeyInCubby(keys - 1); //place key in cubby at the corresponding index
        }
    }

    public void PlaceKeyInCubby(int keyIndex)
    {
        if (_cubbyController != null && _cubbyController.IsSlotAvailable(keyIndex))
        {
            _cubbyController.PlaceKey(keyIndex);
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