using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using Cinemachine;

public class MazeBallPuzzle : MonoBehaviour
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
    private CinemachineVirtualCamera _mainCam;
    private CinemachineVirtualCamera _puzzleCam;
    
    [Space(5)]
    
    [Header("Puzzle Scoring")] 
    private float _currentTimer;
    private const float MaxTimeAllowed = 60f; //we will adjust this based on testing with finished puzzle model.
    private PuzzleEscape _puzzleEscape;
    
    [Space(5)]
    
    [Header("Audio")] 
    private AudioSource _audio;
    [SerializeField] private AudioClip invalidButtonSound;
    [SerializeField] private AudioClip puzzleCompletedSound;

    [Space(5)]
    
    [Header("Animation")]
    private Animator _animator;
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int Finish = Animator.StringToHash("Finish");
    
    private bool _canInteract = true;
    private bool _isInPuzzle;
    public bool solved;
    private PlayerDualHandInventory _playerDualHandInventory;
    private bool _firstTime = true;
    private void Awake()
    {
        puzzle = GetComponent<Puzzle>();
        _audio = GetComponent<AudioSource>();
        _puzzleEscape = GetComponent<PuzzleEscape>();
        _animator = GetComponent<Animator>();
        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        _mainCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("MazePuzzleCam").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (_isInPuzzle)
        {
            // Handle the timer
            HandleTimer(true);

            // Tilt the puzzle with arrow keys
            if (Input.GetKey(KeyCode.A))
            {
                DetermineTiltAxis(0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                DetermineTiltAxis(1);
            }
            if (Input.GetKey(KeyCode.W))
            {
                DetermineTiltAxis(2);
            }
            if (Input.GetKey(KeyCode.S))
            {
                DetermineTiltAxis(3);
            }
        }
        else
        {
            // Stop the timer when the puzzle is not active
            HandleTimer(false);
        }

        if (!solved && Input.GetKeyDown(KeyCode.Escape) &&
            (playerInteractableController.IsLookingAtInteractableObject(gameObject) || puzzleUI.activeSelf))
        {
            _puzzleEscape.EscapePressed?.Invoke();
        }
    }

    public void TogglePuzzleUI()
    {
        if (solved)
        {
            _audio.PlayOneShot(puzzleCompletedSound);
            return;
        }
        
        // if (!_playerDualHandInventory.MatchPuzzlePieceInInventory(gameObject)
        //     && LevelCompletionManager.Instance.currentLevelPuzzles.Count != 2)
        // {
        //     Debug.LogError("Player hasn't completed the image puzzle.");
        //     return;
        // }

        _animator.SetTrigger(Start);

        bool isActive = !puzzleUI.activeSelf;
        puzzleUI.SetActive(isActive);
        _isInPuzzle = isActive; // Update the flag when toggling

        firstPersonController.ToggleCanMove();

        if (!isActive)
        {
            ResetRotation();
        }

        ToggleCamera();
    }


    private void ToggleCamera()
    {
        if (_mainCam.Priority > _puzzleCam.Priority)
        {
            _mainCam.Priority = 0;
            _puzzleCam.Priority = 10;
        }
        else
        {
            _mainCam.Priority = 10;
            _puzzleCam.Priority = 0;
        }
    }

    #region Rotation Logic

    private void DetermineTiltAxis(int directionValue)
    {
        if (! solved && _isInPuzzle && TimerIsActive())
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
        if (_canInteract && !solved)
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
        else
        {
            //_audio.PlayOneShot(invalidButtonSound);
        }
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

    #endregion
    

    private bool TimerIsActive()
    {
        return _currentTimer <= MaxTimeAllowed;
    }

    private void UpdatePuzzleUI()
    {
        timeRemainingText.text = "Time Remaining: " + Mathf.CeilToInt(MaxTimeAllowed - _currentTimer);
    }
    
    //puzzle starts with no ball
    //when the player has the maze ball it should start the ball in animation and spawn a ballprefab in the spawn position
    //that ball prefab should be made a child of the mazeBallObj
    //then if the player runs out of time it resets

    public void InitializeBall()
    {
        if (_firstTime)
        {
            _firstTime = false;
            originalMazeBall = Instantiate(pfMazeBall, mazePuzzleBallSpawnPos.position, Quaternion.identity);
            originalMazeBall.transform.SetParent(mazePuzzleObj.transform);
        }
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

    public void Complete()
    {
        _animator.SetTrigger(Finish);
        
        ToggleCamera();
        firstPersonController.ToggleCanMove();
        firstPersonController.canRotate = true;
        //_audio.enabled = false;
        Destroy(puzzleUI);
    }
    
    private void ResetPuzzle()
    {
        OnInteractStartDelay();
        
        Destroy(originalMazeBall);
        GameObject newBall = Instantiate(pfMazeBall, mazePuzzleBallSpawnPos.position, Quaternion.identity);
        newBall.transform.SetParent(mazePuzzleObj.transform);
        
        originalMazeBall = newBall;
        
        _currentTimer = 0;
        UpdatePuzzleUI();
    }

    public void ResetRotation()
    {
        StartCoroutine(ReturnToIdleRotation());
    }

    private IEnumerator ReturnToIdleRotation()
    {
        float lerpTime = 0f;
        float lerpSpeed = 1.0f;
        Quaternion startRot = mazePuzzleObj.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(Vector3.zero);

        while (lerpTime < 1f)
        {
            lerpTime += Time.deltaTime * lerpSpeed;
            mazePuzzleObj.transform.rotation = Quaternion.Lerp(startRot, targetRot, lerpTime);
            yield return null;
        }
    }


    public void OnInteractStartDelay()
    {
        StartCoroutine(DelayInteraction());
    }
    
    private IEnumerator DelayInteraction()
    {
        _canInteract = false;
        yield return new WaitForSeconds(.1f);
        _canInteract = true;
    }
}