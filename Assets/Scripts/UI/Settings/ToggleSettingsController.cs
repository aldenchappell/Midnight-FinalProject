using UnityEngine;
using UnityEngine.UI;

public class ToggleSettingsController : MonoBehaviour
{
    public string settingKey;

    private Toggle _toggle;

    private void Awake()
    {
        InGameSettingsManager.Instance.LoadSettings();
    }

    private void Start()
    {
        _toggle = GetComponent<Toggle>();

        if (InGameSettingsManager.Instance == null) return;

        _toggle.isOn = PlayerPrefs.GetInt(settingKey, 1) == 1;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        InGameSettingsManager.Instance.SetToggleSetting(settingKey, value);
        Debug.Log("Turning " + settingKey + " to " + value);
    }
}