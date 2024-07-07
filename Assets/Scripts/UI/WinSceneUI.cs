using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinSceneUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private SaveData _saveData;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _saveData = SaveSystem.Load();
        
        mainMenuButton.onClick.AddListener(LevelCompletionManager.Instance.ResetPuzzles);
    }
    public void OnMenuButtonPress()
    {
        SceneManager.LoadScene("MAINMENU");
    }
    public void OnQuitButtonPress()
    {
        _saveData.placedKeys.Clear();
        SaveSystem.Save(_saveData);
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
