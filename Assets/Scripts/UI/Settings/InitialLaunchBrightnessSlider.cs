using UnityEngine;
using UnityEngine.UI;

public class InitialLaunchBrightnessSlider : MonoBehaviour
{
    [SerializeField] private Image _image;

    private const string BrightnessPrefKey = "ScreenBrightness"; 
    private const float MinBrightness = .35f;
    private const float MaxBrightness = 1f;

    private void Start()
    {
        float brightness = PlayerPrefs.GetFloat(BrightnessPrefKey, 0.25f); 
        UpdateImageBrightness(brightness);
    }

    public void OnBrightnessSliderValueChanged(float value)
    {
        float clampedBrightness = Mathf.Clamp(value, 0f, 1f); 
        float brightness = Mathf.Lerp(MinBrightness, MaxBrightness, clampedBrightness);
        
        SetBrightness(brightness);
        
        UpdateImageBrightness(brightness);
    }

    private void SetBrightness(float brightness)
    {
        PlayerPrefs.SetFloat(BrightnessPrefKey, brightness);
        PlayerPrefs.Save();
    }

    private void UpdateImageBrightness(float brightness)
    {
        Color imageColor = _image.color;
        imageColor.a = brightness;
        _image.color = imageColor;
    }
}