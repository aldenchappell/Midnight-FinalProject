using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSlider : MonoBehaviour
{
    private Slider _brightnessSlider;
    private TMP_Text _valueText;
    private void Awake()
    {
        _brightnessSlider = GetComponent<Slider>();
        _valueText = GetComponentInChildren<TMP_Text>();
        InGameSettingsManager.Instance.InitializeBrightness();
    }

    private void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat("ScreenBrightness", 0.5f);
        _brightnessSlider.value = savedBrightness;

        _valueText.text = PlayerPrefs.GetFloat("ScreenBrightness").ToString("f1");
    }

    public void OnBrightnessSliderValueChanged(float value)
    {
        InGameSettingsManager.Instance.SetBrightness(value);
        _valueText.text = PlayerPrefs.GetFloat("ScreenBrightness").ToString("f1");
        //Debug.Log(RenderSettings.ambientLight = Color.white * value);
    }
}