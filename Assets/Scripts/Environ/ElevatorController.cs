using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Animator elevatorAnimator;

    public bool isOpened = false;
    public bool playerInElevator = false;
    private bool _levelSelected = false; //ensure only one button can be pressed
    private string _selectedLevelName = "";

    [SerializeField] private float timeBeforeLoadingLevel = 5.0f;

    [SerializeField] private TMP_Text floorIndexText;

    [Header("Audio Parameters")]
    private AudioSource _elevatorAudioSource;
    [SerializeField] private AudioClip elevatorDingSound;
    [SerializeField] private AudioClip elevatorOpeningSound;
    [SerializeField] private AudioClip elevatorClosingSound;
    [SerializeField] private AudioClip elevatorMovingSound;

    private void Awake()
    {
        //elevatorAnimator = GetComponent<Animator>();
        _elevatorAudioSource = GetComponent<AudioSource>();
    }

    public void OpenElevator()
    {
        if (!isOpened)
        {
            isOpened = true;
            elevatorAnimator.SetBool("Open", isOpened);
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
        elevatorAnimator.SetBool("Open", isOpened);
        _elevatorAudioSource.PlayOneShot(elevatorClosingSound);

        yield return new WaitForSeconds(elevatorClosingSound.length);
    }

    public void SelectLevel(int floorIndex)
    {
        if (_levelSelected)
        {
           // Debug.Log("Level already selected. Ignoring button press.");
            return;
        }

        _levelSelected = true;

        switch (floorIndex)
        {
            case 1:
                _selectedLevelName = "LEVEL ONE";
                floorIndexText.text = "1";
                break;
            case 2:
                _selectedLevelName = "LEVEL TWO";
                floorIndexText.text = "2";
                break;
            case 3:
                _selectedLevelName = "LEVEL THREE";
                floorIndexText.text = "3";
                break;
            default:
                Debug.LogError("Invalid floor index selected.");
                _levelSelected = false; //should prevent errors...
                return;
        }
        
        StartCoroutine(StartElevatorRoutine());
    }

    private IEnumerator StartElevatorRoutine()
    {
        _elevatorAudioSource.PlayOneShot(elevatorDingSound);
        yield return new WaitForSeconds(elevatorDingSound.length + .5f);

        _elevatorAudioSource.PlayOneShot(elevatorMovingSound);

        yield return new WaitForSeconds(timeBeforeLoadingLevel);

        SceneManager.LoadScene(_selectedLevelName);
    }
}
