using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public bool GameIsPaused;
    public GameObject pauseMenuUI;
    public GameObject[] playerUIElements;
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
        // cursor state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // pause game
        Time.timeScale = 1f;
        // ui managing
        pauseMenuUI.SetActive(false);
        
        GameIsPaused = false;
        AudioListener.pause = false;
        pauseSFX.Play();

        
        HideBehindDoor[] hiderDoors = FindObjectsOfType<HideBehindDoor>();
        
        foreach (var element in playerUIElements)
        {
            bool shouldBeActive = true;
            
            if (element.gameObject.name.Contains("DOOR") || element.gameObject.name.Contains("Puzzle"))
            {
                shouldBeActive = false;
                
                foreach (var hiderDoor in hiderDoors)
                {
                    if (hiderDoor == null) return;

                    if (hiderDoor.isActive && element.gameObject.name.Contains("DOORHUD"))
                    {
                        shouldBeActive = true;
                        break;
                    }
                }
            }

            element.SetActive(shouldBeActive);
        }
    }

    private void Pause()
    {
        // cursor state
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // pause game
        Time.timeScale = 0f;
        // ui managing
        pauseMenuUI.SetActive(true);
        
        GameIsPaused = true;
        pauseSFX.Play();
        AudioListener.pause = true;

        foreach (var element in playerUIElements)
        {
            element.SetActive(false);
        }
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
        PlayerPrefs.DeleteAll();
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}