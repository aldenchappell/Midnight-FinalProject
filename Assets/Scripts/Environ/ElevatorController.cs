using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Animator elevatorAnimator;

    public bool isLobbyElevator;
    public bool isOpened = false;
    private bool _levelSelected = false; // Ensure only one button can be pressed
    private string _selectedLevelName = "";

    [SerializeField] private Transform lobbySpawnPosition;
    
    [SerializeField] private float timeBeforeLoadingLevel = 5.0f;

    [SerializeField] private TMP_Text floorIndexText;
    [SerializeField] private TMP_Text invalidLevelText;
    
    [Header("Audio Parameters")]
    private AudioSource _elevatorAudioSource;
    [SerializeField] private AudioClip elevatorDingSound;
    [SerializeField] private AudioClip elevatorOpeningSound;
    [SerializeField] private AudioClip elevatorClosingSound;
    [SerializeField] private AudioClip elevatorMovingSound;
    [SerializeField] private AudioClip invalidLevelSound;

    private static readonly int Floor = Animator.StringToHash("Floor");
    private static readonly int Open = Animator.StringToHash("Open");

    private Coroutine _fadeOutCoroutine;
    private Coroutine _closeElevatorCoroutine;
    private Coroutine _startElevatorRoutineCoroutine;

    [SerializeField] private Animator playerAnim;
    [SerializeField] private GameObject elevatorUI;
    private void Awake()
    {
        _elevatorAudioSource = GetComponent<AudioSource>();

        if (isLobbyElevator && SceneTransitionManager.PreviouslyLoadedSceneName != "MAINMENU")
        {
            GameObject player = GameObject.Find("Player");
            player.transform.position = lobbySpawnPosition.position;
            player.transform.localRotation = lobbySpawnPosition.localRotation;
        }
    }

    private void Start()
    {
        Invoke(nameof(OpenElevator), 1f);
        
        ShowElevatorLevelOnStart();
    }

    public void OpenElevator()
    {
        if (!isOpened)
        {
            isOpened = true;
            elevatorAnimator.SetBool(Open, isOpened);
            
            if (SceneManager.GetActiveScene().name != "LOBBY")
            {
                _elevatorAudioSource.PlayOneShot(elevatorOpeningSound, 1.5f);
            }
            else
            {
                _elevatorAudioSource.PlayOneShot(elevatorOpeningSound);
            }
        }
    }

    public void CloseElevator()
    {
        if (isOpened && _closeElevatorCoroutine == null)
        {
            _closeElevatorCoroutine = StartCoroutine(CloseElevatorRoutine());
            
            ToggleElevatorButtons();
        }
    }

    public void ToggleElevatorButtons()
    {
        elevatorUI.SetActive(!elevatorUI.activeSelf);
    }

    private void ShowElevatorLevelOnStart()
    {
        if (GetLevelName().Contains("LOBBY"))
        {
            floorIndexText.text = "L";
        }
        else if (GetLevelName().Contains("ONE"))
        {
            floorIndexText.text = "1";
        }
        else if (GetLevelName().Contains("TWO"))
        {
            floorIndexText.text = "2";
        }
        else
        {
            floorIndexText.text = "3";
        }
    }

    private string GetLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private IEnumerator CloseElevatorRoutine()
    {
        isOpened = false;
        elevatorAnimator.SetBool(Open, isOpened);
        _elevatorAudioSource.PlayOneShot(elevatorClosingSound);

        yield return new WaitForSeconds(elevatorClosingSound.length);
        _closeElevatorCoroutine = null;
    }

    public void PlayLevelEndAnimation()
    {
        if(!_levelSelected && _startElevatorRoutineCoroutine == null)
            StartCoroutine(WaitForLevelEndAnimation());
    }

    private IEnumerator WaitForLevelEndAnimation()
    {
        yield return new WaitForSeconds(timeBeforeLoadingLevel - 2.0f);
        playerAnim.SetTrigger("End");
    }
    
    public void SelectLevel(int floorIndex)
    {
        // Check if player is in the lobby without enough keys
        if (GetLevelName() == "LOBBY" && LevelCompletionManager.Instance.GetCollectedKeys() >= 1)
        {
            PromptKeyPlacement(true);
            return;
        }

        // Check if player is not in the lobby and has no keys
        if (GetLevelName() != "LOBBY" && LevelCompletionManager.Instance.GetCollectedKeys() <= 0)
        {
            PromptKeyPlacement(false);
            return;
        }

        // Determine the selected level based on floor index
        switch (floorIndex)
        {
            case 1:
                // If already on lobby, show message and return
                if (GetLevelName() == "LOBBY")
                {
                    _elevatorAudioSource.PlayOneShot(invalidLevelSound);
                    FadeText("You are already on this floor.");
                    return;
                }
                _selectedLevelName = "LOBBY";
                break;
            case 2:
                _selectedLevelName = "FLOOR ONE";
                break;
            case 3:
                if (LevelCompletionManager.Instance.IsLevelCompleted("FLOOR ONE"))
                {
                    _selectedLevelName = "FLOOR TWO";
                }
                else
                {
                    _elevatorAudioSource.PlayOneShot(invalidLevelSound);
                    FadeText("You must complete FLOOR ONE first.");
                    return;
                }
                break;
            case 4:
                if (LevelCompletionManager.Instance.IsLevelCompleted("FLOOR ONE") &&
                    LevelCompletionManager.Instance.IsLevelCompleted("FLOOR TWO"))
                {
                    _selectedLevelName = "FLOOR THREE";
                }
                else
                {
                    _elevatorAudioSource.PlayOneShot(invalidLevelSound);
                    FadeText("You must complete FLOOR ONE and FLOOR TWO first.");
                    return;
                }
                break;
            default:
                _elevatorAudioSource.PlayOneShot(invalidLevelSound);
                FadeText("You do not have permission to access this floor.");
                return;
        }
        

        // Check if the selected level is already completed
        if (LevelCompletionManager.Instance.IsLevelCompleted(_selectedLevelName))
        {
            _elevatorAudioSource.PlayOneShot(invalidLevelSound);
            FadeText("This level is already completed. Please select a different level.");
            return;
        }

        
        _levelSelected = true;

        SetElevatorFloorText();
        
        elevatorAnimator.SetInteger(Floor, floorIndex - 1);
        
        _startElevatorRoutineCoroutine ??= StartCoroutine(StartElevatorRoutine());

        // Close elevator
        CloseElevator();
        PlayLevelEndAnimation();
    }

    private void SetElevatorFloorText()
    {
        if (_selectedLevelName == null) return;
        //NOTE: NOT WORKING BECAUSE ALL UI ELEMENTS GET DISABLED
        //WHEN THE ELEVATOR CLOSES
        
        if (_selectedLevelName == "LOBBY")
        {
            floorIndexText.text = "L";
        }
        else if (_selectedLevelName == "FLOOR ONE")
        {
            floorIndexText.text = "1";
        }
        else if (_selectedLevelName == "FLOOR TWO")
        {
            floorIndexText.text = "2";
        }
        else if (_selectedLevelName == "FLOOR THREE")
        {
            floorIndexText.text = "3";
        }
        else
        {
            _selectedLevelName = "L";
        }
    }
    
    public void PromptKeyPlacement(bool isLobby)
    {
        _elevatorAudioSource.PlayOneShot(invalidLevelSound);
        FadeText(isLobby
            ? "You must return the key before entering a new level."
            : "You must collect the key before returning to the lobby.");
    }
    
    private IEnumerator StartElevatorRoutine()
    {
        _elevatorAudioSource.PlayOneShot(elevatorDingSound);
        yield return new WaitForSeconds(elevatorDingSound.length + 0.5f);

        _elevatorAudioSource.PlayOneShot(elevatorMovingSound);
        StartCoroutine(WaitForLevelEndAnimation());
        
        yield return new WaitForSeconds(timeBeforeLoadingLevel - 1);
        
        SceneTransitionManager.UpdatePreviouslyLoadedScene(SceneManager.GetActiveScene().name);

        
        SceneManager.LoadScene(_selectedLevelName);
        _startElevatorRoutineCoroutine = null;
    }

    public void FadeText(string textElement)
    {
        if (_fadeOutCoroutine == null)
        {
            _fadeOutCoroutine = StartCoroutine(FadeOutText(2.0f));
        }
        invalidLevelText.text = textElement;
    }
    
    private IEnumerator FadeOutText(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);

        // How long it will take for the fade to finish
        float fadeDuration = 1.0f;
        float elapsedTime = 0.0f;
            
        Color initialColor = invalidLevelText.color;

        // Gradually increase the alpha value
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / fadeDuration);
                
            invalidLevelText.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        invalidLevelText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
        invalidLevelText.text = "";

        _fadeOutCoroutine = null;
    }
}