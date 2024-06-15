using System;
using UnityEngine;

public class InGameSettingsManager : MonoBehaviour
{
    public static InGameSettingsManager Instance;
    
    [Header("In Game Settings")] 
    public bool enableViewBobbing;
    public bool enableFootstepSounds;
    public bool enableHeartbeatSounds;
    
    
    //for testing purposes only
    public bool enableJumping = false;
    public bool enableDroppingItems;

    [Space(10)]
    
    
    [Header("Custom KeyBinds")]
    public KeyCode objectInteractionKeyOne = KeyCode.E;
    public KeyCode objectInteractionKeyTwo = KeyCode.Mouse0;
    public KeyCode itemExaminationInteractionKey = KeyCode.F;
    public KeyCode exitInteractionKey = KeyCode.Mouse1;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode dropCurrentItem;
    
    
    private const string BrightnessPrefKey = "ScreenBrightness";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad((gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Tab))
        {
            //GlobalCursorManager.Instance.EnableCursor();
        }
    }

    public void ToggleViewBobbing(bool enable)
    {
        enableViewBobbing = enable;
    }
    
    public void ToggleHeartbeat(bool enable)
    {
        enableHeartbeatSounds = enable;
    }
    
    public void SetBrightness(float value)
    {
        RenderSettings.ambientLight = Color.white * value;
        PlayerPrefs.SetFloat(BrightnessPrefKey, value);
        PlayerPrefs.Save();
    }
    
    public void InitializeBrightness()
    {
        float brightness = PlayerPrefs.GetFloat(BrightnessPrefKey, 0.5f); // Default brightness is 0.5
        SetBrightness(brightness);
    }

    public void ToggleFootsteps(bool enable)
    {
        enableFootstepSounds = enable;
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
        int level = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
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
}
