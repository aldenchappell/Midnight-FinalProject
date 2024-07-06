using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinSceneUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        mainMenuButton.onClick.AddListener(LevelCompletionManager.Instance.ResetPuzzles);
    }
    public void OnMenuButtonPress()
    {
        SceneManager.LoadScene("MAINMENU");
    }
    public void OnQuitButtonPress()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
