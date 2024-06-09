using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cinemachine;

public class PhotoBoardPuzzle : MonoBehaviour
{
    [SerializeField] private GameObject startingPuzzleUI;

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

    private GameObject _playerUI;
    private CinemachineVirtualCamera _mainCam;
    private CinemachineVirtualCamera _puzzleCam;

    private PlayerDualHandInventory _playerDualHandInventory;
    private PuzzlePiece _puzzlePieceRequired;
    private PuzzleEscape _puzzleEscape;

    public int polaroidCount;
    private const int TargetPolaroidCount = 6;
    private void Awake()
    {
        polaroidCount = 0;
        _puzzleAudio = GetComponent<AudioSource>();
        _puzzle = GetComponent<Puzzle>();

        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        _puzzlePieceRequired = GetComponent<PuzzlePiece>();
        _puzzleEscape = GetComponent<PuzzleEscape>();
        _firstPersonController = FindObjectOfType<FirstPersonController>();
        _playerUI = GameObject.Find("PlayerUICanvas");
        _mainCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("InFrontPuzzleCam").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && startingPuzzleUI.activeSelf)
        {
            _puzzleEscape.EscapePressed?.Invoke();
        }
    }

    private void Start()
    {
        _movesMade = 0;
        _originalPositions = new List<Vector3>();

        foreach (PhotoBoardPuzzlePiece piece in puzzlePieces)
        {
            _originalPositions.Add(piece.GetPosition);
            print(piece.name + " " + piece.GetPosition);
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
            
        
        if(!_solved
           && polaroidCount != TargetPolaroidCount 
           && LevelCompletionManager.Instance.currentLevelPuzzles.Count != 1 
           && !_playerDualHandInventory.MatchPuzzlePieceInInventory(gameObject))
        {
            Debug.LogError("Player does not have polaroid");
            _puzzleAudio.PlayOneShot(incorrectSlotSound);
            return;
        }

        bool isPuzzleActive = !startingPuzzleUI.activeSelf;
        startingPuzzleUI.SetActive(isPuzzleActive);
        
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
    }

    private void ExitPuzzle()
    {
        GlobalCursorManager.Instance.DisableCursor();
        _playerUI.SetActive(true);
        startingPuzzleUI.SetActive(false);
        
        _firstPersonController.canMove = true;
        _firstPersonController.controller.enabled = true;
        
        ToggleCamera();
    }
    
    private void ToggleCamera()
    {
        if (_mainCam.Priority > _puzzleCam.Priority)
        {
            _mainCam.Priority = 0;
            _puzzleCam.gameObject.SetActive(true);
            _puzzleCam.Priority = 10;
        }
        else
        {
            _mainCam.Priority = 10;
            _puzzleCam.Priority = 0;
            _puzzleCam.gameObject.SetActive(false);
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
}
