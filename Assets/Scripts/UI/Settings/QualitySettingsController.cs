using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualitySettingsController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown, qualityDropdown;
    private Resolution[] _resolutions;

    private void Start()
    {
        _resolutions = Screen.resolutions;
        InitializeResolutionDropdown();
        InitializeQualityDropdown();
        
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", _resolutions.Length - 1);
        SetFirstTimeRes();
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

        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        if (savedResolutionIndex == -1 || savedResolutionIndex >= _resolutions.Length)
        {
            currentResolutionIndex = _resolutions.Length - 1; // Default to highest resolution
        }
        else
        {
            currentResolutionIndex = savedResolutionIndex;
        }
        
        Debug.Log("Saved Resolution Index: " + savedResolutionIndex);
        Debug.Log("Applying Resolution: " + _resolutions[currentResolutionIndex].width + " x " + _resolutions[currentResolutionIndex].height);
        
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        Screen.SetResolution(_resolutions[currentResolutionIndex].width, _resolutions[currentResolutionIndex].height, Screen.fullScreen);
    }

    private void InitializeQualityDropdown()
    {
        int savedQualityLevel = InGameSettingsManager.Instance.GetQualityLevel();
        qualityDropdown.value = savedQualityLevel;
        qualityDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < _resolutions.Length)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Debug.Log("Setting Resolution BLa: " + resolution.width + " x " + resolution.height);
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            InGameSettingsManager.Instance._resolutionIndex = resolutionIndex;
            print(InGameSettingsManager.Instance._resolutionIndex);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("Invalid resolution index: " + resolutionIndex);
        }
    }

    public void SetQuality(int qualityLevel)
    {
        InGameSettingsManager.Instance.SetQualityLevel(qualityLevel);
        Debug.Log("Current Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public void SetFirstTimeRes()
    {
        if(!InGameSettingsManager.Instance.hasSetFirstTime)
        {
            print("Setting first");
            SetResolution(_resolutions.Length - 1);
            InGameSettingsManager.Instance.hasSetFirstTime = true;
        }
    }
}