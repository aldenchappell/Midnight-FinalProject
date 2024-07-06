using UnityEngine;

public class CheckFirstLaunch : MonoBehaviour
{
    private void Start()
    {
        CheckForFirstLaunch();
    }
    
    private void CheckForFirstLaunch()
    {
        if (InGameSettingsManager.Instance.isFirstLaunch)
            InGameSettingsManager.Instance.isFirstLaunch = false;
    }
}