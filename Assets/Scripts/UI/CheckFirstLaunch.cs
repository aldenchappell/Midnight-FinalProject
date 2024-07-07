using UnityEngine;

public class CheckFirstLaunch : MonoBehaviour
{
    private SaveData _saveData;
    
    private void Awake()
    {
        CheckForFirstLaunch();
    
        _saveData = SaveSystem.Load();

        // if (!InGameSettingsManager.Instance.isFirstLaunch)
        // {
        //     LevelCompletionManager.Instance.ResetGame();
        // }
    }


    private void CheckForFirstLaunch()
    {
        if (!InGameSettingsManager.Instance.isFirstLaunch)
        {
            InGameSettingsManager.Instance.isFirstLaunch = true;
            //clear the key cubby on first launch just to make sure.
            _saveData.placedKeys.Clear();
            SaveSystem.Save(_saveData);
        }
    }
}