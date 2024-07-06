using UnityEngine;
using UnityEngine.SceneManagement;

//written by Creature Sari
//edited by Alden Chappell
public class UIMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject startGamePanel;
    public void StartGameButton()
    {
        GlobalCursorManager.Instance.DisableCursor();

        if (InGameSettingsManager.Instance.isFirstLaunch)
        {
            startGamePanel.SetActive(true);
            GlobalCursorManager.Instance.EnableCursor();
        }
        else
        {
            LoadLobby();
        }
    }

    private void LoadLobby()
    {
        SceneManager.LoadScene("LOBBY");
        GlobalCursorManager.Instance.DisableCursor();
    }

    public void ExitButton()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
