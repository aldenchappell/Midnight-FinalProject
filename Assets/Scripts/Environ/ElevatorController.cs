using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Animator elevatorAnimator;

    public bool isOpened = false;
    private bool _levelSelected = false; // Ensure only one button can be pressed
    private string _selectedLevelName = "";

    [SerializeField] private float timeBeforeLoadingLevel = 5.0f;

    [SerializeField] private TMP_Text floorIndexText;

    [Header("Audio Parameters")]
    private AudioSource _elevatorAudioSource;
    [SerializeField] private AudioClip elevatorDingSound;
    [SerializeField] private AudioClip elevatorOpeningSound;
    [SerializeField] private AudioClip elevatorClosingSound;
    [SerializeField] private AudioClip elevatorMovingSound;
    [SerializeField] private AudioClip invalidLevelSound;

    private static readonly int Floor = Animator.StringToHash("Floor");
    private static readonly int Open = Animator.StringToHash("Open");

    private void Awake()
    {
        _elevatorAudioSource = GetComponent<AudioSource>();
    }

    public void OpenElevator()
    {
        if (!isOpened)
        {
            isOpened = true;
            elevatorAnimator.SetBool(Open, isOpened);
            _elevatorAudioSource.PlayOneShot(elevatorOpeningSound);
        }
    }

    public void CloseElevator()
    {
        if (isOpened)
        {
            StartCoroutine(CloseElevatorRoutine());
        }
    }

    private IEnumerator CloseElevatorRoutine()
    {
        isOpened = false;
        elevatorAnimator.SetBool(Open, isOpened);
        _elevatorAudioSource.PlayOneShot(elevatorClosingSound);

        yield return new WaitForSeconds(elevatorClosingSound.length);
    }

    public void SelectLevel(int floorIndex)
    {
        if (_levelSelected)
        {
            _elevatorAudioSource.PlayOneShot(invalidLevelSound);
            Debug.Log("A level is already being loaded. Ignoring button press.");
            return;
        }

        switch (floorIndex)
        {
            case 1:
                _selectedLevelName = "LOBBY";
                break;
            case 2:
                _selectedLevelName = "LEVEL ONE";
                break;
            case 3:
                _selectedLevelName = "LEVEL TWO";
                break;
            case 4:
                _selectedLevelName = "LEVEL THREE";
                break;
            case 5:
                _selectedLevelName = "ALDEN";
                break;
            default:
                _elevatorAudioSource.PlayOneShot(invalidLevelSound);
                Debug.LogError("Invalid floor index selected.");
                return;
        }

        // Check if the level is already completed
        if (LevelCompletionManager.Instance.IsLevelCompleted(_selectedLevelName))
        {
            _elevatorAudioSource.PlayOneShot(invalidLevelSound);
            Debug.Log("This level is already completed. Please select a different level.");
        }
        else
        {
            _levelSelected = true;
            floorIndexText.text = floorIndex.ToString();
            elevatorAnimator.SetInteger(Floor, floorIndex);

            Debug.Log(floorIndex);
            StartCoroutine(StartElevatorRoutine());
        }
    }

    private IEnumerator StartElevatorRoutine()
    {
        _elevatorAudioSource.PlayOneShot(elevatorDingSound);
        yield return new WaitForSeconds(elevatorDingSound.length + 0.5f);

        _elevatorAudioSource.PlayOneShot(elevatorMovingSound);

        yield return new WaitForSeconds(timeBeforeLoadingLevel);

        SceneManager.LoadScene(_selectedLevelName);
    }
}