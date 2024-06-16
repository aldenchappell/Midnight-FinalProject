using UnityEngine;
using UnityEngine.UI;

public class FootstepsToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = InGameSettingsManager.Instance.enableFootstepSounds;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool newValue)
    {
        InGameSettingsManager.Instance.ToggleFootsteps(newValue);
    }
}