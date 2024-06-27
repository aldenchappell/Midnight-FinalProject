using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Slider slider;
    [SerializeField] private string volumeParameter;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float volumeMultiplier = 30f;

    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void Start()
    {
        InGameSettingsManager.Instance.LoadSettings(); // Ensure settings are loaded
        float savedVolume = InGameSettingsManager.Instance.GetVolume(volumeParameter, 0.5f); // Default to 0.5f
        slider.value = savedVolume;
        HandleSliderValueChanged(savedVolume); // Update mixer and text based on initial value
    }

    private void HandleSliderValueChanged(float value)
    {
        if (value <= 0f)
        {
            mixer.SetFloat(volumeParameter, -80f); // Muted
            volumeText.text = "0";
        }
        else
        {
            mixer.SetFloat(volumeParameter, Mathf.Log10(value) * volumeMultiplier);
            volumeText.text = value.ToString("f1");
        }

        InGameSettingsManager.Instance.SetVolume(volumeParameter, value); // Update PlayerPrefs
    }

    // Function to load volume externally if needed
    public void LoadVolume()
    {
        float savedVolume = InGameSettingsManager.Instance.GetVolume(volumeParameter, 0.5f); // Default to 0.5f
        slider.value = savedVolume;
        HandleSliderValueChanged(savedVolume); // Update mixer and text based on loaded value
    }
}
