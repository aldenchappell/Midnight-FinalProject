using UnityEngine;
using UnityEngine.UI;

public class HeartbeatToggle : MonoBehaviour
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
        InGameSettingsManager.Instance.enableHeartbeatSounds = newValue;
        var heartBeatAudio = GameObject.Find("HeartbeatAudio").GetComponent<AudioSource>();
        if(heartBeatAudio != null)
            heartBeatAudio.enabled = newValue;
        Debug.Log(newValue);
    }
}