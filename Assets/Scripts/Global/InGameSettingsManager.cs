using UnityEngine;

public class InGameSettingsManager : MonoBehaviour
{
    public static InGameSettingsManager Instance;


    [Header("In Game Settings")] 
    public bool enableViewBobbing;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad((gameObject));
        }
        else
        {
            Destroy((gameObject));
        }
    }
    
    
    
}
