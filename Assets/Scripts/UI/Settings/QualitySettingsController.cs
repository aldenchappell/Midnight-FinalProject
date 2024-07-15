using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualitySettingsController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown, qualityDropdown;
    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions;
    private float _currentRefreshRate;
    private int _currentResolutionIndex = 0;

    [SerializeField] private GameObject[] disableOnRuntimeObjects;
    [SerializeField] private GameObject soundSettings;
    [Obsolete("Obsolete")]
    private void Start()
    {
        #region Resolution Setup
        _resolutions = Screen.resolutions;
        _filteredResolutions = new List<Resolution>();
        
        resolutionDropdown.ClearOptions();
        _currentRefreshRate = Screen.currentResolution.refreshRate;

        //Debug.Log("Refresh Rate: " + _currentRefreshRate);

        for (int i = 0; i < _resolutions.Length; i++)
        {
            //Debug.Log("Resolution: " + _resolutions[i]);
            if (_resolutions[i].refreshRate == _currentRefreshRate)
            {
                _filteredResolutions.Add(_resolutions[i]);
            }
        }
        
        List<string> options = new List<string>();
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string resolutionOption = _filteredResolutions[i].width + "x" + _filteredResolutions[i].height + " " +
                                      _filteredResolutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);
            if (_filteredResolutions[i].width == Screen.width && _filteredResolutions[i].height == Screen.height)
            {
                _currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = _currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        #endregion

        LoadSettings();

        DisableSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < _filteredResolutions.Count)
        {
            Resolution res = _filteredResolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, true);
            PlayerPrefs.SetInt("Resolution", resolutionIndex);
            PlayerPrefs.Save();
        
            //Debug.Log("Setting resolution to " + res.width + "x" + res.height);
        }
    }


    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        
        //Debug.Log("Setting quality to " + PlayerPrefs.GetInt("QualityLevel"));
    }

    private void LoadSettings()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt("Resolution", -1);

        if (savedResolutionIndex == -1)
        {
            // Default resolution to 1920x1080
            for (int i = 0; i < _filteredResolutions.Count; i++)
            {
                if (_filteredResolutions[i].width == 1920 && _filteredResolutions[i].height == 1080)
                {
                    savedResolutionIndex = i;
                    PlayerPrefs.SetInt("Resolution", savedResolutionIndex);
                    PlayerPrefs.Save();
                    break;
                }
            }
        }

        SetResolution(savedResolutionIndex);
        SetQuality(PlayerPrefs.GetInt("QualityLevel", 2));
        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel");
        
        InGameSettingsManager.Instance.InitializeVolumes();
        InGameSettingsManager.Instance.InitializeBrightness();
        InGameSettingsManager.Instance.SetMouseSensitivity(PlayerPrefs.GetFloat("MouseSensitivity", 100.0f));


        // Debug.Log("Loading settings. Quality level is " + QualitySettings.GetQualityLevel() + "\n" + "Resolution is "
        //           + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
    }


    private void DisableSettings()
    {
        foreach (var obj in disableOnRuntimeObjects)
        {
            if(obj != null)
                obj.SetActive(false);
        }
        if(soundSettings != null)
            soundSettings.SetActive(false);
    }
    
    #region Old Settings

    // private void Start()
    // {
    //     _resolutions = Screen.resolutions;
    //     InitializeResolutionDropdown();
    //     InitializeQualityDropdown();
    //
    //     SetFirstTimeRes();
    //
    //     // Add listener to quality dropdown
    //     qualityDropdown.onValueChanged.AddListener(OnQualityDropdownValueChanged);
    // }
    //
    // private void InitializeResolutionDropdown()
    // {
    //     resolutionDropdown.ClearOptions();
    //     List<string> options = new List<string>();
    //     int currentResolutionIndex = 0;
    //     for (int i = 0; i < _resolutions.Length; i++)
    //     {
    //         string option = _resolutions[i].width + " x " + _resolutions[i].height;
    //         options.Add(option);
    //         if (_resolutions[i].width == Screen.currentResolution.width &&
    //             _resolutions[i].height == Screen.currentResolution.height)
    //         {
    //             currentResolutionIndex = i;
    //         }
    //     }
    //     resolutionDropdown.AddOptions(options);
    //
    //     int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
    //     if (savedResolutionIndex == -1 || savedResolutionIndex >= _resolutions.Length)
    //     {
    //         currentResolutionIndex = _resolutions.Length - 1; // Default to highest resolution
    //     }
    //     else
    //     {
    //         currentResolutionIndex = savedResolutionIndex;
    //     }
    //     
    //     // Apply the saved or default resolution
    //     resolutionDropdown.value = currentResolutionIndex;
    //     resolutionDropdown.RefreshShownValue();
    //     Screen.SetResolution(_resolutions[currentResolutionIndex].width, _resolutions[currentResolutionIndex].height, Screen.fullScreen);
    // }
    //
    // private void InitializeQualityDropdown()
    // {
    //     qualityDropdown.ClearOptions();
    //     List<string> options = new List<string>();
    //     foreach (string name in QualitySettings.names)
    //     {
    //         options.Add(name);
    //     }
    //     qualityDropdown.AddOptions(options);
    //
    //     int savedQualityLevel = InGameSettingsManager.Instance.GetQualityLevel();
    //     qualityDropdown.value = savedQualityLevel;
    //     qualityDropdown.RefreshShownValue();
    // }
    //
    // private void OnQualityDropdownValueChanged(int qualityLevel)
    // {
    //     SetQuality(qualityLevel);
    // }
    //
    // public void SetResolution(int resolutionIndex)
    // {
    //     if (resolutionIndex >= 0 && resolutionIndex < _resolutions.Length)
    //     {
    //         Resolution resolution = _resolutions[resolutionIndex];
    //         Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    //
    //         InGameSettingsManager.Instance.resolutionIndex = resolutionIndex;
    //         PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    //         PlayerPrefs.Save();
    //     }
    // }
    //
    // public void SetQuality(int qualityLevel)
    // {
    //     if (qualityLevel >= 0 && qualityLevel < QualitySettings.names.Length)
    //     {
    //         QualitySettings.SetQualityLevel(qualityLevel, true);
    //         InGameSettingsManager.Instance.SetQualityLevel(qualityLevel);
    //     }
    // }
    //
    // private void SetFirstTimeRes()
    // {
    //     if (!InGameSettingsManager.Instance.hasSetFirstTime)
    //     {
    //         SetResolution(_resolutions.Length - 1);
    //         InGameSettingsManager.Instance.hasSetFirstTime = true;
    //         PlayerPrefs.SetInt("HasSetFirstTime", 1);
    //         PlayerPrefs.Save();
    //     }
    // }

    #endregion
}