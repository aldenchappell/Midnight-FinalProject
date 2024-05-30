using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WoodBlockPuzzle : MonoBehaviour
{
    [SerializeField] private GameObject startingPuzzleUI;
    
    private bool solved = false;
    [SerializeField] private FirstPersonController firstPersonController;

    private WoodBlockPuzzlePiece _currentSelectedPuzzlePiece;

    [SerializeField] private WoodBlockPuzzlePiece[] puzzlePieces;
    [SerializeField] private WoodBlockPuzzleSlot[] puzzleSlots;

    [SerializeField] private TMP_Text movesMadeText;
    private int _movesMade;
    private int _maxMoves = 6;

    private AudioSource _puzzleAudio;
    [SerializeField] private AudioClip correctSlotSound;
    [SerializeField] private AudioClip incorrectSlotSound;

    private void Awake()
    {
        _puzzleAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _movesMade = 0;
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
            //update the level completion manager and save this puzzle as completed.
            
            //Destroy the puzzle. (TO DO)
            //LevelCompletionManager.Instance.DestroyPuzzle(gameObject);
        }
        else
        {
            if (_movesMade == _maxMoves)
            {
                Debug.Log("Puzzle lost. Resetting puzzle...");
                TogglePuzzleUI();
                //ResetPuzzle();
            }
        }
    }
    
    private void ResetPuzzle()
    {
        // Reset moves counter
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