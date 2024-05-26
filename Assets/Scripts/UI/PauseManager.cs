using UnityEngine;
using UnityEngine.SceneManagement;

//Creature Sari

public class PauseManager : MonoBehaviour
{
    public bool GameIsPaused;
    public GameObject pauseMenuUI;
    public GameObject playerUI;
    public AudioSource pauseSFX;

    private void Start()
    {
        GameIsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //cursor state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //pause game
        Time.timeScale = 1f;
        //ui managing
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        GameIsPaused = false;
        AudioListener.pause = false;
        pauseSFX.Play();

    }

    public void Pause()
    {
        //cursor state
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //pause game
        Time.timeScale = 0f;
        //ui managing
        pauseMenuUI.SetActive(true);
        playerUI.SetActive(false);
        GameIsPaused = true;
        pauseSFX.Play();
        AudioListener.pause = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Loading Menu...");
        SceneManager.LoadScene("MAINMENU");
        AudioListener.pause = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
