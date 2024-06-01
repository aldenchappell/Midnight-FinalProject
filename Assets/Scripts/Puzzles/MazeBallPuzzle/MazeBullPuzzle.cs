using StarterAssets;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;

public class MazeBullPuzzle : MonoBehaviour
{
    [HideInInspector] public Puzzle puzzle;

    [SerializeField] private PlayerInteractableController playerInteractableController;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private GameObject mazePuzzleObj;
    [SerializeField] private GameObject originalMazeBall;
    [SerializeField] private GameObject pfMazeBall;
    [SerializeField] private Transform mazePuzzleBallSpawnPos;
    
    [Header("Tilting")]
    [SerializeField] private float tiltSpeed;
    [SerializeField] private float minAngle, maxAngle;
    
    [Space(5)]
    
    [Header("UI")] 
    [SerializeField] private GameObject puzzleUI;
    [SerializeField] private TMP_Text timeRemainingText;

    [Space(5)]
    
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera puzzleCam;
    
    [Space(5)]
    
    [Header("Puzzle Scoring")] 
    private float _currentTimer;
    private const float MaxTimeAllowed = 60f; //we will adjust this based on testing with finished puzzle model.

    [Space(5)]
    
    [Header("Audio")] 
    private AudioSource _audio;

    private bool _isInPuzzle;
    
    private void Awake()
    {
        puzzle = GetComponent<Puzzle>();
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (mazePuzzleObj.activeSelf)
        {
            // Handle the timer
            HandleTimer(true);

            // Tilt the puzzle with arrow keys
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                DetermineTiltAxis(0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                DetermineTiltAxis(1);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                DetermineTiltAxis(2);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                DetermineTiltAxis(3);
            }
        }
        else
        {
            // Stop the timer when the puzzle is not active
            HandleTimer(false);
        }

        if (Input.GetKeyDown(KeyCode.F) &&
            (playerInteractableController.IsLookingAtInteractableObject(mazePuzzleObj) || mazePuzzleObj.activeSelf))
        {
            TogglePuzzleUI();
        }
    }

    public void TogglePuzzleUI()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);

        bool isActive = puzzleUI.activeSelf;
        _isInPuzzle = isActive; // Update the flag when toggling

        firstPersonController.ToggleCanMove();
        
        mazePuzzleObj.SetActive(isActive);
        
        if (isActive)
        {
            SwitchToPuzzleCamera();
        }
        else
        {
            SwitchToPlayerCamera();
            //_currentTimer = 0; // Reset the timer when exiting the puzzle
        }
    }

    private void SwitchToPuzzleCamera()
    {
        playerCam.Priority = 0;
        puzzleCam.Priority = 10;
    }

    private void SwitchToPlayerCamera()
    {
        playerCam.Priority = 10;
        puzzleCam.Priority = 0;
    }

    // Determine the direction the player chooses to tilt the puzzle
    private void DetermineTiltAxis(int directionValue)
    {
        if (mazePuzzleObj.activeSelf && TimerIsActive())
        {
            // Tilt the maze puzzle object
            switch (directionValue)
            {
                case 0:
                    TiltPuzzle(Vector3.forward, 10); // Rotate around the forward (positive Z-axis)
                    break;
                case 1:
                    TiltPuzzle(Vector3.forward, -10); // Rotate around the backward (negative Z-axis)
                    break;
                case 2:
                    TiltPuzzle(Vector3.left, -10); // Rotate around the left (negative X-axis)
                    break;
                case 3:
                    TiltPuzzle(Vector3.left, 10); // Rotate around the right (positive X-axis)
                    break;
                default:
                    Debug.LogError("Error tilting the maze puzzle. MazeBullPuzzle/DetermineTiltAxis");
                    break;
            }
        }
    }

    // Limit the angle the puzzle can tilt and tilt the puzzle
    private void TiltPuzzle(Vector3 axis, float angle)
    {
        // Calculate the new rotation angle
        Vector3 newRotation = mazePuzzleObj.transform.rotation.eulerAngles + axis * angle;
        
        newRotation.x = NormalizeTiltAngle(newRotation.x);
        newRotation.z = NormalizeTiltAngle(newRotation.z);

        // Clamp the rotation
        newRotation.x = Mathf.Clamp(newRotation.x, minAngle, maxAngle);
        newRotation.z = Mathf.Clamp(newRotation.z, minAngle, maxAngle);

        // Rotate the puzzle using tilt speed
        mazePuzzleObj.transform.rotation = Quaternion.Lerp(
            mazePuzzleObj.transform.rotation,
            Quaternion.Euler(newRotation),
            Time.deltaTime * tiltSpeed);
    }

    // Normalize the tilt angle to ensure it stays within 0 and 360
    private float NormalizeTiltAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    private bool TimerIsActive()
    {
        return _currentTimer <= MaxTimeAllowed;
    }

    private void UpdatePuzzleUI()
    {
        timeRemainingText.text = "Time Remaining: " + Mathf.CeilToInt(MaxTimeAllowed - _currentTimer);
    }

    private void HandleTimer(bool isActive)
    {
        if (isActive)
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer >= MaxTimeAllowed)
            {
                ResetPuzzle();
            }
        }
        
        UpdatePuzzleUI();
    }

    private void ResetPuzzle()
    {
        Destroy(originalMazeBall);
        GameObject newBall = Instantiate(pfMazeBall, mazePuzzleBallSpawnPos.position, Quaternion.identity);
        newBall.transform.SetParent(mazePuzzleObj.transform);
        newBall.GetComponent<MazeBall>().mazePuzzle = this;
        
        originalMazeBall = newBall;
        
        // Reset the timer
        _currentTimer = 0;
        UpdatePuzzleUI();
    }
}
