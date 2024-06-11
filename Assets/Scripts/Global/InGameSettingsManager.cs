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
    public KeyCode objectInteractionKeyOne;
    public KeyCode objectInteractionKeyTwo;
    public KeyCode crouchKey;
    public KeyCode sprintKey;
    public KeyCode swapInventorySlotKey;
    public KeyCode dropCurrentItem;
    
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

    public void ToggleViewBobbing()
    {
        enableViewBobbing = !enableViewBobbing;
    }

    public void ToggleFootsteps()
    {
        enableFootstepSounds = !enableFootstepSounds;
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
}
