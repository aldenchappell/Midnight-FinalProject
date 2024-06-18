using TMPro;
using UnityEngine;

public class PlayerKeyController : MonoBehaviour
{
    [SerializeField] private TMP_Text keysCollectedText;
    [SerializeField] private TMP_Text pauseMenuKeysCollectedText;
    public int keys;
    
    private void Start()
    {
        keys = 0;
    }

    // This method will be the event that occurs when the player interacts with a key.
    public void CollectKey()
    {
        keys++;
        UpdateKeyUI();
    }

    private void UpdateKeyUI()
    {
        keysCollectedText.text = keys.ToString();
        pauseMenuKeysCollectedText.text = "Keys collected: " + keys;
    }
}
