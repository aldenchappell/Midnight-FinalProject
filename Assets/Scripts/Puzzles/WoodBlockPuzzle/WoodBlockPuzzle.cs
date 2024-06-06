using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WoodBlockPuzzle : MonoBehaviour
{
    [SerializeField] private GameObject startingPuzzleUI;

    //Added by Owen
    [SerializeField] Sprite originalSlotSprite;
    
    private bool solved = false;
    [SerializeField] private FirstPersonController firstPersonController;

    private WoodBlockPuzzlePiece _currentSelectedPuzzlePiece;

    [SerializeField] private WoodBlockPuzzlePiece[] puzzlePieces;
    private List<Vector3> originalPositions;
    [SerializeField] private WoodBlockPuzzleSlot[] puzzleSlots;

    [SerializeField] private TMP_Text movesMadeText;
    private int _movesMade;
    private int _maxMoves = 6;

    private AudioSource _puzzleAudio;
    [SerializeField] private AudioClip correctSlotSound;
    [SerializeField] private AudioClip incorrectSlotSound;


    private Puzzle _puzzle;
    private void Awake()
    {
        _puzzleAudio = GetComponent<AudioSource>();

        _puzzle = GetComponent<Puzzle>();
    }

    private void Start()
    {
        _movesMade = 0;
        originalPositions = new List<Vector3>();
        //Owen:
        //Get all pieces original positions
        foreach (WoodBlockPuzzlePiece piece in puzzlePieces)
        {
            originalPositions.Add(piece.GetPosition);
            print(piece.name + " " + piece.GetPosition);
        }
        UpdateMovesMadeUI(_movesMade);
    }

    public void PlacePuzzlePiece(Button slotButton)
    {
        if (solved) return;
        WoodBlockPuzzleSlot slot = slotButton.GetComponent<WoodBlockPuzzleSlot>();
        if (slot != null && _currentSelectedPuzzlePiece != null)
        {
            // Increment moves made
            _movesMade++;
            UpdateMovesMadeUI(_movesMade);

            if (slot.value == _currentSelectedPuzzlePiece.puzzlePieceID)
            {
                // Place the puzzle piece in the slot
                slot.puzzlePiece = _currentSelectedPuzzlePiece;
                slotButton.GetComponent<Image>().sprite = _currentSelectedPuzzlePiece.GetComponent<Image>().sprite;

                // Disable the piece button to prevent multiple placements
                _currentSelectedPuzzlePiece.GetComponent<Button>().interactable = false;

                // Clear the current selection
                _currentSelectedPuzzlePiece = null;

                // Check if the puzzle is solved
                CheckForPuzzleCompletion();

                _puzzleAudio.PlayOneShot(correctSlotSound);
            }
            else
            {
                // Player connected an incorrect piece
                CheckForPuzzleCompletion();
                _puzzleAudio.PlayOneShot(incorrectSlotSound);
                Debug.Log("Wrong slot");
            }
        }
    }


    private void CheckForPuzzleCompletion()
    {
        
        if (puzzleSlots[0].GetComponent<Image>().sprite == puzzlePieces[0].GetComponent<Image>().sprite
            && puzzleSlots[1].GetComponent<Image>().sprite == puzzlePieces[1].GetComponent<Image>().sprite
            && puzzleSlots[2].GetComponent<Image>().sprite == puzzlePieces[2].GetComponent<Image>().sprite
            && puzzleSlots[3].GetComponent<Image>().sprite == puzzlePieces[3].GetComponent<Image>().sprite)
        {
            Debug.Log("Puzzle completed");
            solved = true;
            TogglePuzzleUI();
            ResetPuzzle();//testing
            
            // Update the level completion manager and save this puzzle as completed.
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
            if (_movesMade == _maxMoves)
            {
                Debug.Log("Puzzle lost. Resetting puzzle...");
                TogglePuzzleUI();
                ResetPuzzle();
            }
        }
    }
    
    private void ResetPuzzle()
    {
        //Owen:
        //Return all pieces to original positions
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            puzzlePieces[i].GetComponent<RectTransform>().anchoredPosition = originalPositions[i];
            puzzlePieces[i].GetComponent<Button>().interactable = true;
        }
        foreach(WoodBlockPuzzleSlot slot in puzzleSlots)
        {
            slot.GetComponent<Image>().sprite = originalSlotSprite;
        }

        _movesMade = 0;
        UpdateMovesMadeUI(_movesMade);
        
    }
    

    public void TogglePuzzleUI()
    {
        bool isPuzzleActive = !startingPuzzleUI.activeSelf;
        startingPuzzleUI.SetActive(isPuzzleActive);

        // Toggle canMove and cursor visibility based on puzzle activity
        firstPersonController.canMove = !isPuzzleActive;
        firstPersonController.controller.enabled = !isPuzzleActive;
        if (isPuzzleActive)
        {
            GlobalCursorManager.Instance.EnableCursor();
        }
        else
        {
            GlobalCursorManager.Instance.DisableCursor();
        }
    }

    public void SelectPuzzlePiece(Button puzzlePieceButton)
    {
        if(solved) return;
        _currentSelectedPuzzlePiece = puzzlePieceButton.GetComponent<WoodBlockPuzzlePiece>();
    }

    private void UpdateMovesMadeUI(int movesUsed)
    {
        movesMadeText.text = "Moves Made: " + movesUsed + "/" + _maxMoves;
    }
}