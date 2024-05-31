using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class MazeBullPuzzle : MonoBehaviour
{
    [HideInInspector] public Puzzle puzzle;

    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private GameObject mazePuzzleObj;
    [SerializeField] private MazeBall mazeBall;

    [SerializeField] private float tiltSpeed;
    [SerializeField] private float minAngle, maxAngle;
    
    
    
    [Header("UI")] 
    [SerializeField] private GameObject puzzleUI;
    [SerializeField] private TMP_Text timeRemainingText;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera puzzleCam;
    
    [Header("Puzzle Scoring")] 
    private int _currentTimer;
    private int _maxTimeAllowed;
    
    [Header("Audio")] 
    private AudioSource _audio;
    
    private void Awake()
    {
        puzzle = GetComponent<Puzzle>();
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (mazePuzzleObj.activeSelf)
        {
            //tilt the puzzle with arrow keys
            
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
    }

    public void TogglePuzzleUI()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);

        bool isActive = puzzleUI.activeSelf;
        
        firstPersonController.ToggleCanMove();
        
        mazePuzzleObj.SetActive(isActive);
        
        if (isActive)
        {
            SwitchToPuzzleCamera();
        }
        else
        {
            SwitchToPlayerCamera();
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

    //determine the direction the player chooses to tilt the puzzle
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
    

    //limit the angle the puzzle can tilt and tilt the puzzle
    private void TiltPuzzle(Vector3 axis, float angle)
    {
        //calculate the rotation angle
        Vector3 newRotation = mazePuzzleObj.transform.rotation.eulerAngles + axis * angle;
        
        newRotation.x = NormalizeTiltAngle(newRotation.x);
        newRotation.z = NormalizeTiltAngle(newRotation.z);

        //clamp the rotation
        newRotation.x = Mathf.Clamp(newRotation.x, minAngle, maxAngle);
        newRotation.z = Mathf.Clamp(newRotation.z, minAngle, maxAngle);

        //rotate the puzzle using tilt speed
        mazePuzzleObj.transform.rotation = Quaternion.Lerp(
            mazePuzzleObj.transform.rotation,
            Quaternion.Euler(newRotation),
            Time.deltaTime * tiltSpeed);
    }

    //normalize the tilt angle to ensure it stays within 0 and 360
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
        return _currentTimer <= _maxTimeAllowed;
    }

    private void UpdatePuzzleUI()
    {
        timeRemainingText.text = "Time Remaining: " + _currentTimer;
    }
}
