using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualityDropdown : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown, qualityDropdown;
    private Resolution[] _resolutions;
    
    private void Start()
    {
        _resolutions = Screen.resolutions;
        InitializeResolutionDropdown();
        InitializeQualityDropdown();
    }

    private void InitializeResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        
        // Get saved resolution index
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitializeQualityDropdown()
    {
        // Get saved quality level
        int savedQualityLevel = InGameSettingsManager.Instance.GetQualityLevel();

        // Set the dropdown value to the saved quality level
        qualityDropdown.value = savedQualityLevel;
        qualityDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        
        // Save resolution index
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityLevel)
    {
        InGameSettingsManager.Instance.SetQualityLevel(qualityLevel);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
