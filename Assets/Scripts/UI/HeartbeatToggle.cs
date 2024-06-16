using UnityEngine;
using UnityEngine.UI;

public class HeartbeatToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = InGameSettingsManager.Instance.enableHeartbeatSounds;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool newValue)
    {
        InGameSettingsManager.Instance.ToggleHeartbeat(newValue);
        var heartBeatAudio = GameObject.Find("HeartbeatAudio").GetComponent<AudioSource>();
        if (heartBeatAudio != null)
            heartBeatAudio.enabled = newValue;
    }
}