using UnityEngine;
using UnityEngine.UI;

public class InitialLaunchBrightnessSlider : MonoBehaviour
{
    [SerializeField] private Image _image;

    private const string BrightnessPrefKey = "ScreenBrightness"; // Adjust if different in your settings manager
    private const float MinBrightness = .6f;
    private const float MaxBrightness = 1f;

    private void Start()
    {
        // Initialize brightness based on saved settings
        float brightness = PlayerPrefs.GetFloat(BrightnessPrefKey, 0.25f); // Default to 0.25 if not set
        UpdateImageBrightness(brightness);
    }

    public void OnBrightnessSliderValueChanged(float value)
    {
        float clampedBrightness = Mathf.Clamp(value, 0f, 1f); // Assuming slider value is normalized (0 to 1)
        float brightness = Mathf.Lerp(MinBrightness, MaxBrightness, clampedBrightness);

        // Update brightness in settings manager
        SetBrightness(brightness);

        // Update image alpha based on brightness
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