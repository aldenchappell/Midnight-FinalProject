using UnityEngine;
using UnityEngine.UI;

public class BrightnessSlider : MonoBehaviour
{
    private Slider _brightnessSlider;

    private void Awake()
    {
        _brightnessSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat("ScreenBrightness", 0.5f);
        _brightnessSlider.value = savedBrightness;
    }


    public void OnBrightnessSliderValueChanged(float value)
    {
        //Debug.Log("Brightness slider value changed: " + value);
        InGameSettingsManager.Instance.SetBrightness(value);
    }
}