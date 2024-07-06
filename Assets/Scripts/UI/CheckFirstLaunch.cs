using UnityEngine;

public class CheckFirstLaunch : MonoBehaviour
{
    private void Awake()
    {
        CheckForFirstLaunch();
    }

    private void CheckForFirstLaunch()
    {
        if (!InGameSettingsManager.Instance.isFirstLaunch)
            InGameSettingsManager.Instance.isFirstLaunch = true;
    }
}