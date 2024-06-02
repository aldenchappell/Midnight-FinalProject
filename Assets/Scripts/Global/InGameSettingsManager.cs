using UnityEngine;

public class InGameSettingsManager : MonoBehaviour
{
    public static InGameSettingsManager Instance;
    
    [Header("In Game Settings")] 
    public bool enableViewBobbing;
    public bool enableFootstepSounds;
    public bool enableHeartbeatSounds;
    
    //for testing purposes only
    public bool enableJumping;

    [Space(10)]
    
    
    [Header("Custom KeyBinds")]
    public KeyCode objectInteractionKey;
    public KeyCode crouchKey;
    public KeyCode sprintKey;
    public KeyCode swapInventorySlotKey;
    
    
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

    public void ToggleViewBobbing()
    {
        enableViewBobbing = !enableViewBobbing;
    }

    public void ToggleFootsteps()
    {
        enableFootstepSounds = !enableFootstepSounds;
    }
}
