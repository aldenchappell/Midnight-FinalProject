using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public Slider sensitivitySlider;

    void Start()
    {
        sensitivitySlider.minValue = InGameSettingsManager.Instance.minMouseSensitivity;
        sensitivitySlider.maxValue = InGameSettingsManager.Instance.maxMouseSensitivity;
        sensitivitySlider.value = InGameSettingsManager.Instance.GetMouseSensitivity();

        sensitivitySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        InGameSettingsManager.Instance.SetMouseSensitivity(value);
    }
}