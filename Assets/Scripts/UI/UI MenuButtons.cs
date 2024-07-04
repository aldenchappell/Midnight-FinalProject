using UnityEngine;
using UnityEngine.SceneManagement;

//written by Creature Sari
public class UIMenuButtons : MonoBehaviour
{
    public void StartGameButton()
    {
        SceneManager.LoadScene("LOBBY");
        GlobalCursorManager.Instance.DisableCursor();
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
