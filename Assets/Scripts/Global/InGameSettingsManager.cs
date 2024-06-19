using System;
using UnityEngine;

public class InGameSettingsManager : MonoBehaviour
{
    public static InGameSettingsManager Instance;
    
    [ColoredHeader("In Game Settings", "#FF00FF")] 
    public bool enableHeadBobbing;
    public bool enableFootstepSounds;
    public bool enableHeartbeatSounds;
    public bool enableCompass = true;
    
    //for testing purposes only
    public bool enableJumping = false;
    public bool enableDroppingItems;

    [Space(10)]
    
    
    [ColoredHeader("Custom KeyBinds", "#FFFF00")]
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
        float brightness = PlayerPrefs.GetFloat(BrightnessPrefKey, .5f); // Default brightness is 0.5
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
    
    private void LoadSettings()
    {
        enableHeadBobbing = PlayerPrefs.GetInt(ViewBobbingPrefKey, 1) == 1;
        enableFootstepSounds = PlayerPrefs.GetInt(FootstepSoundsPrefKey, 1) == 1;
        enableHeartbeatSounds = PlayerPrefs.GetInt(HeartbeatSoundsPrefKey, 1) == 1;
        enableCompass = PlayerPrefs.GetInt(HeartbeatSoundsPrefKey, 1) == 1;
        SetQualityLevel(GetQualityLevel());
    }
}