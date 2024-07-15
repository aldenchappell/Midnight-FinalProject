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
        
        mainMenuButton.onClick.AddListener(() => LevelCompletionManager.Instance.ResetGame(false));
    }
    public void OnMenuButtonPress()
    {
        //SceneManager.LoadScene("MAINMENU");
        Loader.Load(Loader.Scene.MAINMENU);
    }
    public void OnQuitButtonPress()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
