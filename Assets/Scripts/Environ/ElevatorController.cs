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
    private void Awake()
    {
        _elevatorAudioSource = GetComponent<AudioSource>();

        if (isLobbyElevator && SceneTransitionManager.PreviouslyLoadedSceneName != "MAINMENU")
        {
            GameObject player = GameObject.Find("Player");
            player.transform.position = lobbySpawnPosition.position;
            player.transform.localRotation = lobbySpawnPosition.localRotation;
        }

        //SceneTransitionManager.UpdatePreviouslyLoadedScene(SceneManager.GetActiveScene().name);
        //Debug.Log(SceneTransitionManager.PreviouslyLoadedSceneName);
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
        }
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
        StartCoroutine(WaitForLevelEndAnimation());
    }

    private IEnumerator WaitForLevelEndAnimation()
    {
        yield return new WaitForSeconds(timeBeforeLoadingLevel - 1.0f);
        var playerAnim = GameObject.FindObjectOfType<PlayerKeyController>().GetComponent<Animator>();
        playerAnim.SetTrigger("End");
    }
    
    public void SelectLevel(int floorIndex)
    {
        if (_levelSelected)
        {
            _elevatorAudioSource.PlayOneShot(invalidLevelSound);
            FadeText("A level is already being loaded");
            return;
        }

        if (LevelCompletionManager.Instance.hasKey)
        {
            _elevatorAudioSource.PlayOneShot(invalidLevelSound);
            FadeText("You must return the key before entering a new level.");
            return;
        }

        switch (floorIndex)
        {
            case 1:
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

        // Check if the level is already completed
        if (LevelCompletionManager.Instance.IsLevelCompleted(_selectedLevelName))
        {
            _elevatorAudioSource.PlayOneShot(invalidLevelSound);
            FadeText("This level is already completed. Please select a different level.");
            
        }
        else if(floorIndex != 1)
        {
            _levelSelected = true;
            floorIndexText.text = (floorIndex - 1).ToString();
            elevatorAnimator.SetInteger(Floor, floorIndex - 1);
            
            if (_startElevatorRoutineCoroutine == null)
            {
                _startElevatorRoutineCoroutine = StartCoroutine(StartElevatorRoutine());
                StartCoroutine(CloseElevatorRoutine());
            }
        }
        else
        {
            floorIndexText.text = "L";
            if (_startElevatorRoutineCoroutine == null)
            {
                _startElevatorRoutineCoroutine = StartCoroutine(StartElevatorRoutine());
                StartCoroutine(CloseElevatorRoutine());
            }
        }
    }


    private IEnumerator StartElevatorRoutine()
    {
        _elevatorAudioSource.PlayOneShot(elevatorDingSound);
        yield return new WaitForSeconds(elevatorDingSound.length + 0.5f);

        _elevatorAudioSource.PlayOneShot(elevatorMovingSound);

        yield return new WaitForSeconds(timeBeforeLoadingLevel);
        
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