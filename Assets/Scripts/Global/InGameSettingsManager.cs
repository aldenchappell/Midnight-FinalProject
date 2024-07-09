using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InGameSettingsManager : MonoBehaviour
{
    public static InGameSettingsManager Instance;
    
    //[ColoredHeader("In Game Settings", "#FF00FF")] 
    public bool enableHeadBobbing = true;
    public bool enableFootstepSounds = true;
    public bool enableHeartbeatSounds = true;
    public bool enableCompass = true;
    public bool enableShaking = true;
    
    //for testing purposes only
    public bool enableJumping = false;
    public bool enableDroppingItems;

    [Space(10)]
    
    public bool isFirstLaunch = true;
    
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
    private const string ShakePrefKey = "Shake";
    
    //Sensitivity
    public float minMouseSensitivity = 10.0f;
    public float maxMouseSensitivity = 200.0f;
    
    private PostProcessVolume _postProcessVolume;
    private AutoExposure _autoExposure;
    
    //Brightness
    private const float MinBrightness = 0.05f;
    private const float MaxBrightness = .75f;
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
    
    /// <summary>
    /// PUT THIS METHOD ON EVERY TOGGLE.
    /// APPLY THE TOGGLESETTINGSCONTROLLER TO THE TOGGLE AS WELL.
    /// GO TO THE TOP OF THIS SCRIPT WHERE ALL OF THE 'PREF' KEYS ARE DECLARED
    /// IF THERE ISNT A SETTING FOR THE DESIRED TOGGLE, DECLARE A NEW ONE AND MAKE ANOTHER CASE BELOW!
    /// Questions? Ask Alden.
    /// </summary>
    /// <param name="settingKey"></param>
    /// <param name="value"></param>
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
            // case ShakePrefKey:
            //     enableShaking = value;
            //     break;
            default: Debug.Log("Error setting toggle setting.");
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
        float clampedBrightness = Mathf.Clamp(value, MinBrightness, MaxBrightness);
        RenderSettings.ambientLight = new Color(clampedBrightness, clampedBrightness, clampedBrightness, 1);
        PlayerPrefs.SetFloat(BrightnessPrefKey, clampedBrightness);
        PlayerPrefs.Save();
    }
    
    private void InitializeBrightness()
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
        InitializeBrightness();
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