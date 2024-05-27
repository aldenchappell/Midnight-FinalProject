using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Animator elevatorAnimator;

    private bool _isOpened = false;
    
    [SerializeField] private float timeBeforeLoadingLevel = 5.0f;

    [SerializeField] private TMP_Text floorIndexText;
    
    
    [Header("Audio Parameters")]
    private AudioSource _elevatorAudioSource;
    [SerializeField] private AudioClip elevatorDingSound;
    [SerializeField] private AudioClip elevatorOpeningSound;
    [SerializeField] private AudioClip elevatorClosingSound;
    private void Awake()
    {
        elevatorAnimator = GetComponent<Animator>();
        
        _elevatorAudioSource = GetComponent<AudioSource>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
    }

    //Used for interaction
    public void OpenElevator()
    {
        if (!_isOpened)
        {
            _isOpened = true;
            elevatorAnimator.SetBool("Open", _isOpened);
            //Play a door opening sound
            _elevatorAudioSource.PlayOneShot(elevatorOpeningSound);
        }
    }

    //This method will be attached to the buttons in the elevator and will determine which level the player is
    //brought to.
    public void CloseElevator(string levelName)
    {
        if(_isOpened)
            StartCoroutine(LoadLevelAfterClosingAnimation(levelName));
    }

    private IEnumerator LoadLevelAfterClosingAnimation(string levelName)
    {
        //close the elevator
        elevatorAnimator.SetBool("Open", false);
        //Play a door closing sound
        _elevatorAudioSource.PlayOneShot(elevatorClosingSound);
        
        //Play an elevator ding sound when the elevator goes up
        //Idea - *ding* ... "going up"
        yield return new WaitForSeconds(2.0f);
        _elevatorAudioSource.PlayOneShot(elevatorDingSound);
        
        //Assign the elevator level text
        AssignElevatorFloorText(levelName);
        
        yield return new WaitForSeconds(timeBeforeLoadingLevel);
        SceneManager.LoadScene(levelName);
    }

    private void AssignElevatorFloorText(string levelName)
    {
        switch (levelName)
        {
            case "LEVEL ONE":
                floorIndexText.text = "1";
                break;
            case "LEVEL TWO":
                floorIndexText.text = "2";
                break;
            case "LEVEL THREE":
                floorIndexText.text = "3";
                break;
            default: Debug.Log("Error assigning the elevator floor text. Invalid level name?" +
                               " ElevatorController/AssignElevatorFloorText");
                break;
        }
    }
}
