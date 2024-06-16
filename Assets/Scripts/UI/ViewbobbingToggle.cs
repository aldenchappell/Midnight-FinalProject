using UnityEngine;
using UnityEngine.UI;

public class ViewbobbingToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = InGameSettingsManager.Instance.enableViewBobbing;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool newValue)
    {
        InGameSettingsManager.Instance.ToggleViewBobbing(newValue);
    }
}