using UnityEngine;
using UnityEngine.SceneManagement;

//written by Creature Sari
public class UIMenuButtons : MonoBehaviour
{
    public void TestSceneButton()
    {
        SceneManager.LoadScene("TEST SCENE");
        GlobalCursorManager.Instance.DisableCursor();
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
