using UnityEngine;
using UnityEngine.UI;

public class HeadbobToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = InGameSettingsManager.Instance.enableHeadBobbing;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool newValue)
    {
        InGameSettingsManager.Instance.ToggleHeadBobbing(newValue);
    }
}