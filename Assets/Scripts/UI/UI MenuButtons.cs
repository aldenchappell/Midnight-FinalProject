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

        if (PlayerPrefs.GetInt("FirstLaunch", 0) == 1)
        {
            startGamePanel.SetActive(true);
            GlobalCursorManager.Instance.EnableCursor();
        }
        else
        {
            LoadLobby();
        }
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("LOBBY");
        GlobalCursorManager.Instance.DisableCursor();
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
