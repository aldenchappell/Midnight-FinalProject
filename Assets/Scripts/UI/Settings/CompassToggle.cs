using UnityEngine;
using UnityEngine.UI;

public class CompassToggle : MonoBehaviour
{
    private Toggle _toggle;
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();

        _toggle.isOn = InGameSettingsManager.Instance.enableCompass;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);

    }

    private void OnToggleValueChanged(bool value)
    {
        InGameSettingsManager.Instance.ToggleCompass(value);
    }
}
