using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool newValue)
    {
        // Toggle the enableHeartbeatSounds boolean in the InGameSettingsManager
        Screen.fullScreen = newValue;
        Debug.Log(newValue);
    }
}