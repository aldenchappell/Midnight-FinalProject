using System;
using UnityEngine;

public class InGameSettingsManager : MonoBehaviour
{
    public static InGameSettingsManager Instance;
    
    //[ColoredHeader("In Game Settings", "#FF00FF")] 
    public bool enableHeadBobbing;
    public bool enableFootstepSounds;
    public bool enableHeartbeatSounds;
    public bool enableCompass = true;
    
    //for testing purposes only
    public bool enableJumping = false;
    public bool enableDroppingItems;

    [Space(10)]
    
    
    //[ColoredHeader("Custom KeyBinds", "#FFFF00")]
    public KeyCode objectInteractionKeyOne = KeyCode.E;
    public KeyCode objectInteractionKeyTwo = KeyCode.Mouse0;
    public KeyCode itemExaminationInteractionKey = KeyCode.F;
    public KeyCode exitInteractionKey = KeyCode.Mouse1;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;
    
    public KeyCode dropCurrentItem;
    
    private const string BrightnessPrefKey = "ScreenBrightness";
    private const string ViewBobbingPrefKey = "ViewBobbing";
    private const string FootstepSoundsPrefKey = "Footsteps";
    private const string HeartbeatSoundsPrefKey = "Heartbeat";
    private const string CompassPrefKey = "Compass";
    
    //Sensitivity
    public float minMouseSensitivity = 10.0f;
    public float maxMouseSensitivity = 200.0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad((gameObject));
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckFirstLaunch();
    }
    
    void CheckFirstLaunch()
    {
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            Debug.Log("First Launch");
            PlayerPrefs.DeleteAll(); 
            PlayerPrefs.SetInt("FirstLaunch", 1); 
            PlayerPrefs.Save(); 
        }
        
    }
    
    public float GetMouseSensitivity()
    {
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100.0f);
        sensitivity = Mathf.Clamp(sensitivity, minMouseSensitivity, maxMouseSensitivity);
        return sensitivity;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        float clampedSensitivity = Mathf.Clamp(sensitivity, minMouseSensitivity, maxMouseSensitivity);
        PlayerPrefs.SetFloat("MouseSensitivity", clampedSensitivity);
        PlayerPrefs.Save();
    }
    
    
    public void SetToggleSetting(string settingKey, bool value)
    {
        PlayerPrefs.SetInt(settingKey, value ? 1 : 0);
        PlayerPrefs.Save();

        switch (settingKey)
        {
            case ViewBobbingPrefKey:
                enableHeadBobbing = value;
                break;
            case FootstepSoundsPrefKey:
                enableFootstepSounds = value;
                break;
            case HeartbeatSoundsPrefKey:
                enableHeartbeatSounds = value;
                break;
            case CompassPrefKey:
                enableCompass = value;
                PlayerCompassController compassController = FindObjectOfType<PlayerCompassController>();
                if (compassController)
                {
                    compassController.SetCompassAlpha(value ? 1 : 0);
                }
                break;
        }
    }

    public void ToggleCompass(bool enable)
    {
        enableCompass = enable;
        PlayerPrefs.SetInt(CompassPrefKey, enable ? 1 : 0);
        PlayerPrefs.Save();

        PlayerCompassController compassController = FindObjectOfType<PlayerCompassController>();
        if (compassController != null)
        {
            compassController.SetCompassAlpha(enable ? 1 : 0);
        }
    }

    public void ToggleHeadBobbing(bool enable)
    {
        enableHeadBobbing = enable;
        PlayerPrefs.SetInt(ViewBobbingPrefKey, enable ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void ToggleHeartbeat(bool enable)
    {
        enableHeartbeatSounds = enable;
        PlayerPrefs.SetInt(HeartbeatSoundsPrefKey, enable ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void SetBrightness(float value)
    {
        RenderSettings.ambientLight = Color.white * value;
        PlayerPrefs.SetFloat(BrightnessPrefKey, value);
        PlayerPrefs.Save();
    }
    
    public void InitializeBrightness()
    {
        float brightness = PlayerPrefs.GetFloat(BrightnessPrefKey, .25f);
        SetBrightness(brightness);
    }

    public void ToggleFootsteps(bool enable)
    {
        enableFootstepSounds = enable;
        PlayerPrefs.SetInt(FootstepSoundsPrefKey, enable ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    public float GetVolume(string parameter, float defaultValue)
    {
        float value = PlayerPrefs.GetFloat(parameter, defaultValue);
        return value;
    }
    
    public void SetVolume(string parameter, float value)
    {
        PlayerPrefs.SetFloat(parameter, value);
        PlayerPrefs.Save();
    }

    public int GetQualityLevel()
    {
        int level = PlayerPrefs.GetInt("QualityLevel", UnityEngine.QualitySettings.GetQualityLevel());
        return level;
    }

    public void SetQualityLevel(int level)
    {
        PlayerPrefs.SetInt("QualityLevel", level);
        QualitySettings.SetQualityLevel(level);
        PlayerPrefs.Save();
    }

    public void ToggleHeartbeat()
    {
        enableHeartbeatSounds = !enableHeartbeatSounds;
        Debug.Log(enableHeartbeatSounds);
    }
    public void LoadSettings()
    {
        enableHeadBobbing = PlayerPrefs.GetInt(ViewBobbingPrefKey, 1) == 1;
        enableFootstepSounds = PlayerPrefs.GetInt(FootstepSoundsPrefKey, 1) == 1;
        enableHeartbeatSounds = PlayerPrefs.GetInt(HeartbeatSoundsPrefKey, 1) == 1;
        enableCompass = PlayerPrefs.GetInt(CompassPrefKey, 1) == 1;
        
        SetQualityLevel(GetQualityLevel());
        InitializeBrightness();
        InitializeVolumes(); 
    }

    private void InitializeVolumes()
    {
        SetVolume(BrightnessPrefKey, PlayerPrefs.GetFloat(BrightnessPrefKey, 0.5f));
        SetVolume(ViewBobbingPrefKey, PlayerPrefs.GetFloat(ViewBobbingPrefKey, 0.5f));
        SetVolume(FootstepSoundsPrefKey, PlayerPrefs.GetFloat(FootstepSoundsPrefKey, 0.5f));
        SetVolume(HeartbeatSoundsPrefKey, PlayerPrefs.GetFloat(HeartbeatSoundsPrefKey, 0.5f));
        SetVolume(CompassPrefKey, PlayerPrefs.GetFloat(CompassPrefKey, 0.5f));
    }
}