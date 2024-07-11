using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//written by Creature Sari
//edited by Alden Chappell
public class UIMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject startGamePanel;
    private static bool isFirstTimeLobbyLoaded = true;

    public void StartGameButton()
    {
        GlobalCursorManager.Instance.DisableCursor();

        if (InGameSettingsManager.Instance.isFirstLaunch && isFirstTimeLobbyLoaded)
        {
            startGamePanel.SetActive(true);
            GlobalCursorManager.Instance.EnableCursor();
            isFirstTimeLobbyLoaded = false; 
        }
        else
        {
            LoadLobby();
        }
    }

    private void LoadLobby()
    {
        StartCoroutine(FadeThenLoad());
    }

    private IEnumerator FadeThenLoad()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("LOBBY");
        GlobalCursorManager.Instance.DisableCursor();
    }

    public void ExitButton()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void LoadUrl(string url)
    {
        Application.OpenURL(url);
    }
}