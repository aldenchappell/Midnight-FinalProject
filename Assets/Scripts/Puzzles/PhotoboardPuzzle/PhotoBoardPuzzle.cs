using System;
using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;

public class PhotoBoardPuzzle : MonoBehaviour, IPlaySkullDialogue
{
    [SerializeField] private GameObject[] puzzleUI;

    //Added by Owen
    [SerializeField] Sprite originalSlotSprite;
    
    [SerializeField] private bool _solved = false;
    private FirstPersonController _firstPersonController;

    private PhotoBoardPuzzlePiece _currentSelectedPuzzlePiece;

    [SerializeField] private PhotoBoardPuzzlePiece[] puzzlePieces;
    private List<Vector3> _originalPositions;
    [SerializeField] private PhotoBoardPuzzleSlot[] puzzleSlots;

    [SerializeField] private TMP_Text movesMadeText;
    private int _movesMade;
    private const int MaxMoves = 10;

    private AudioSource _puzzleAudio;
    [SerializeField] private AudioClip correctSlotSound;
    [SerializeField] private AudioClip incorrectSlotSound;

    private Puzzle _puzzle;
    private PatrolSystemManager _patrol;

    private GameObject _playerUI;
    private CinemachineVirtualCamera _mainCam;
    private CinemachineVirtualCamera _puzzleCam;

    private PlayerInteractableController _playerInteractableController;
    private PlayerDualHandInventory _playerDualHandInventory;
    //private PuzzlePiece _puzzlePieceRequired;
    private PuzzleEscape _puzzleEscape;

    private GameObject _polaroidObj;
    public int polaroidCount;
    private const int TargetPolaroidCount = 6;
    private bool _isFirstTime = true;
    private bool _isInPuzzle = false;
    
    
    private void Awake()
    {
        _puzzleAudio = GetComponent<AudioSource>();
        _puzzle = GetComponent<Puzzle>();
        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        _puzzleEscape = GetComponent<PuzzleEscape>();
        _firstPersonController = FindObjectOfType<FirstPersonController>();
        _playerUI = GameObject.Find("PlayerUICanvas");
        _playerInteractableController = FindObjectOfType<PlayerInteractableController>(); ;
        _mainCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("PhotoBoardPuzzleCam").GetComponent<CinemachineVirtualCamera>();
        _patrol = GameObject.Find("DemonPatrolManager").GetComponent<PatrolSystemManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerInteractableController.IsLookingAtInteractableObject(gameObject))
        {
            _puzzleEscape.EscapePressed?.Invoke();
        }
    }

    private void Start()
    {
        _movesMade = 0;
        polaroidCount = 0;
        _originalPositions = new List<Vector3>();

        foreach (PhotoBoardPuzzlePiece piece in puzzlePieces)
        {
            _originalPositions.Add(piece.GetPosition);
            //print(piece.name + " " + piece.GetPosition);
        }
        UpdateMovesMadeUI(_movesMade);
    }

    public void PlacePuzzlePiece(Button slotButton)
    {
        if (_solved) return;
        PhotoBoardPuzzleSlot slot = slotButton.GetComponent<PhotoBoardPuzzleSlot>();
        if (slot != null && _currentSelectedPuzzlePiece != null)
        {

            _movesMade++;
            UpdateMovesMadeUI(_movesMade);

            if (slot.value == _currentSelectedPuzzlePiece.puzzlePieceID)
            {

                slot.puzzlePiece = _currentSelectedPuzzlePiece;
                slotButton.GetComponent<Image>().sprite = _currentSelectedPuzzlePiece.GetComponent<Image>().sprite;
                var color = slotButton.GetComponent<Image>().color;
                slotButton.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 255f);
                
                _currentSelectedPuzzlePiece.GetComponent<Button>().interactable = false;


                _currentSelectedPuzzlePiece = null;

       
                CheckForPuzzleCompletion();

                _puzzleAudio.PlayOneShot(correctSlotSound);
            }
            else
            {
                CheckForPuzzleCompletion();
                _puzzleAudio.PlayOneShot(incorrectSlotSound);
                Debug.Log("Wrong slot");
            }
        }
    }

    private void CheckForPuzzleCompletion()
    {
        bool isPuzzleComplete = true;
        
        for (int i = 0; i < puzzleSlots.Length; i++)
        {
            if (puzzleSlots[i].GetComponent<Image>().sprite != puzzlePieces[i].GetComponent<Image>().sprite)
            {
                isPuzzleComplete = false;
                break;
            }
        }

        if (isPuzzleComplete)
        {
            Debug.Log("Puzzle completed");
            _solved = true;
            ExitPuzzle();
            TogglePuzzleUI();
            LevelCompletionManager.Instance.OnKeySpawn();
            if (_puzzle != null)
            {
                _puzzle.CompletePuzzle();
            }
            else
            {
                Debug.LogError("Puzzle reference is null!");
            }
        }
        else
        {
            if (_movesMade == MaxMoves)
            {
                Debug.Log("Puzzle lost. Resetting puzzle...");
                TogglePuzzleUI();
                ResetPuzzle();
                _patrol.DecreaseTimeToSpawn = 10;
                _patrol.ReferenceToSuspicion = transform.position;
            }
        }
    }

    private void ResetPuzzle()
    {
        // Return all pieces to original positions
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            puzzlePieces[i].GetComponent<RectTransform>().anchoredPosition = _originalPositions[i];
            puzzlePieces[i].GetComponent<Button>().interactable = true;
        }

        foreach (PhotoBoardPuzzleSlot slot in puzzleSlots)
        {
            slot.GetComponent<Image>().sprite = originalSlotSprite;
        }

        _movesMade = 0;
        UpdateMovesMadeUI(_movesMade);
    }

    public void TogglePuzzleUI()
    {
        if (_solved)
        {
            _puzzleAudio.PlayOneShot(incorrectSlotSound);
            return;
        }
        
        // Check for polaroid only if the player is not already in the puzzle
        if (!_isInPuzzle)
        {
            bool hasPolaroid = _playerDualHandInventory.GetInventory.Any(item => item != null && item.CompareTag("Polaroid"));
            
            if (polaroidCount != TargetPolaroidCount && _isFirstTime && !hasPolaroid)
            {
                PlaySpecificSkullDialogueClip(
                    SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.solveImageAndMazeBallPuzzlesClip);
                return;
            }
            
            if (polaroidCount != TargetPolaroidCount)
            {
                PlaySpecificSkullDialogueClip(
                    SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.collectPolaroidsClip);
                Debug.Log("Player hasn't collected all of the polaroids.");
                return;
            }
            
            if (_isFirstTime && !hasPolaroid)
            {
                _puzzleAudio.PlayOneShot(incorrectSlotSound);
                PlaySpecificSkullDialogueClip(
                    SkullDialogueLineHolder.Instance.audioSource,
                    SkullDialogueLineHolder.Instance.solveMazeBallPuzzleClip);
                Debug.LogError("Player doesn't have the polaroid.");
                return;
            }

            
        }

        // Toggle the puzzle UI
        bool isPuzzleActive = !puzzleUI[0].activeSelf;
        _isInPuzzle = isPuzzleActive;

        foreach (var element in puzzleUI)
        {
            element.SetActive(isPuzzleActive);
        }

        // Toggle canMove and cursor visibility based on puzzle activity
        _firstPersonController.canMove = !isPuzzleActive;
        _firstPersonController.controller.enabled = !isPuzzleActive;

        if (isPuzzleActive)
        {
            GlobalCursorManager.Instance.EnableCursor();
            _playerUI.SetActive(false);
        }
        else
        {
            GlobalCursorManager.Instance.DisableCursor();
            _playerUI.SetActive(true);
        }

        ToggleCamera();
        PlacePolaroid();
    }

    private void ExitPuzzle()
    {
        GlobalCursorManager.Instance.DisableCursor();
        _playerUI.SetActive(true);

        foreach (var elm in puzzleUI)
        {
            elm.SetActive(false);
        }
                
        _firstPersonController.canMove = true;
        _firstPersonController.controller.enabled = true;
        
        ToggleCamera();
    }

    private void PlacePolaroid()
    {
        if (_isFirstTime)
        {
            _isFirstTime = false;
            
            _polaroidObj = GameObject.FindWithTag("Polaroid");
            _playerDualHandInventory.RemoveObject = _polaroidObj;
            _polaroidObj = null;
        }
    }
    
    private void ToggleCamera()
    {
        if (_mainCam.Priority > _puzzleCam.Priority)
        {
            _mainCam.Priority = 0;
          // _puzzleCam.gameObject.SetActive(true);
            _puzzleCam.Priority = 10;
        }
        else
        {
            _mainCam.Priority = 10;
            _puzzleCam.Priority = 0;
           // _puzzleCam.gameObject.SetActive(false);
        }
    }

    public void SelectPuzzlePiece(Button puzzlePieceButton)
    {
        if (_solved) return;
        _currentSelectedPuzzlePiece = puzzlePieceButton.GetComponent<PhotoBoardPuzzlePiece>();
    }

    private void UpdateMovesMadeUI(int movesUsed)
    {
        movesMadeText.text = "Moves Made: " + movesUsed + "/" + MaxMoves;
    }

    public void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            source.PlayOneShot(clip);
        }
            
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            
        }
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            
        }
    }

    public IEnumerator PlaySkullDialoguePuzzleHintClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip)
    {
        yield return null;
    }
}