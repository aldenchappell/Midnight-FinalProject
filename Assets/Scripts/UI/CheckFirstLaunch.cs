using UnityEngine;

public class CheckFirstLaunch : MonoBehaviour
{
    private void Start()
    {
        CheckForFirstLaunch();

        //just to make sure :)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CheckForFirstLaunch()
    {
        if (!InGameSettingsManager.Instance.isFirstLaunch)
            InGameSettingsManager.Instance.isFirstLaunch = true;
    }
}