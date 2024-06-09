using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using Random = UnityEngine.Random;

public class SlidingImagePuzzle : MonoBehaviour
{
    private Puzzle _puzzle;
    
    [SerializeField] private GameObject puzzleUI;

    [SerializeField] private Sprite[] puzzleSprites;
    [SerializeField] private List<Image> gridSlotImages;

    private int _selectedSlotIndex = -1;
    private bool _isFirstSlotSelected;

    
    [SerializeField] private FirstPersonController firstPersonController;

    [Header("Puzzle scoring")]
    [SerializeField] private TMP_Text movesRemainingText;
    private int _movesMade;
    private int _maxMoves = 15;

    private AudioSource _audio;
    [SerializeField] private AudioClip correctMoveSound;
    
    [SerializeField] private GameObject mazeballPrefab;
    private PuzzleEscape _puzzleEscape;
    private void Awake()
    {
        _puzzle = GetComponent<Puzzle>();
        _audio = GetComponent<AudioSource>();
        _puzzleEscape = GetComponent<PuzzleEscape>();
    }


    private void Start()
    {
        AssignUniqueSprites();
        _movesMade = 0;
        UpdatePuzzleUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && puzzleUI.activeSelf)
        {
            _puzzleEscape.EscapePressed?.Invoke();
        }
    }

    public void TogglePuzzleUI()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        
        firstPersonController.canMove = !puzzleUI.activeSelf;
        firstPersonController.controller.enabled = !puzzleUI.activeSelf;
        if (puzzleUI.activeSelf)
        {
            GlobalCursorManager.Instance.EnableCursor();
        }
        else
        {
            GlobalCursorManager.Instance.DisableCursor();
        }
    }
    
    private void AssignUniqueSprites()
    {
        List<Sprite> uniqueSprites = new List<Sprite>(puzzleSprites);

        //shuffle the unique sprites
        for (int i = uniqueSprites.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Sprite tempSprite = uniqueSprites[i];
            uniqueSprites[i] = uniqueSprites[randomIndex];
            uniqueSprites[randomIndex] = tempSprite;
        }

        //assign the unique sprites
        for (int i = 0; i < gridSlotImages.Count; i++)
        {
            gridSlotImages[i].sprite = uniqueSprites[i];
        }
    }

    //onclick() for each slot
    public void OnSlotClicked(int clickedIndex)
    {
        if (!_isFirstSlotSelected)
        {
            _selectedSlotIndex = clickedIndex;
            _isFirstSlotSelected = true;
        }
        //check to make sure the player cant select the same slot, as well as ensure
        //that the second selected slot is adjacent to the first selected slot
        else if (clickedIndex != _selectedSlotIndex && IsAdjacentSlot(_selectedSlotIndex, clickedIndex))
        {
            SwapSlotImages(_selectedSlotIndex, clickedIndex);
            _isFirstSlotSelected = false;
        }
    }

    //ensure that the selected slot and the second slot are adjacent do one another.
    private bool IsAdjacentSlot(int index1, int index2)
    {
        //calculate the row and column for both the first and second selected slots
        int row1 = index1 / 3;
        int col1 = index1 % 3;
        int row2 = index2 / 3;
        int col2 = index2 % 3;

        //if the slots are adjacent (horizontally or vertically)
        return (row1 == row2 && Math.Abs(col1 - col2) == 1) || (col1 == col2 && Mathf.Abs(row1 - row2) == 1);
    }

    
    private void SwapSlotImages(int index1, int index2)
    {
        //check if an invalid index is selected
        if (index1 < 0 || index1 >= gridSlotImages.Count || index2 < 0 || index2 >= gridSlotImages.Count)
        {
            return;
        }

        //check to see if the player has reached the maximum number of moves
        if (_movesMade >= _maxMoves)
        {
            Debug.Log("Out of moves. Resetting puzzle.");
            ResetPuzzle();
            return; 
        }

        // Swap the grid slot images
        Image image1 = gridSlotImages[index1];
        Image image2 = gridSlotImages[index2];

        Sprite tempSprite = image1.sprite;
        image1.sprite = image2.sprite;
        image2.sprite = tempSprite;

        _audio.PlayOneShot(correctMoveSound);

        
        _movesMade++;
        if (_movesMade >= _maxMoves)
        {
            Debug.Log("Reached maximum moves. Resetting puzzle.");
            ResetPuzzle();
            return; 
        }

        UpdatePuzzleUI();
        CheckForFinishedPuzzle();
    }



    private void CheckForFinishedPuzzle()
    {
        if (gridSlotImages[0].sprite == puzzleSprites[0] &&
            gridSlotImages[1].sprite == puzzleSprites[1] &&
            gridSlotImages[2].sprite == puzzleSprites[2] &&
            gridSlotImages[3].sprite == puzzleSprites[3] &&
            gridSlotImages[4].sprite == puzzleSprites[4] &&
            gridSlotImages[5].sprite == puzzleSprites[5] &&
            gridSlotImages[6].sprite == puzzleSprites[6] &&
            gridSlotImages[7].sprite == puzzleSprites[7] &&
            gridSlotImages[8].sprite == puzzleSprites[8])
        {
            _puzzle.CompletePuzzle();
            firstPersonController.ToggleCanMove();
        }
    }

    public void InstantiateMazeballAtPlayerFeet()
    {
        if (mazeballPrefab != null)
        {
            // Get the player's position
            Vector3 playerPosition = firstPersonController.transform.position;

            // Instantiate the mazeball at the player's position
            Instantiate(mazeballPrefab, playerPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Mazeball prefab is not assigned in the inspector.");
        }
    }

    

    private void UpdatePuzzleUI()
    {
        movesRemainingText.text = "Moves Made: " + _movesMade + "/" + _maxMoves;
    }

    private void ResetPuzzle()
    {
        _movesMade = 0;
        UpdatePuzzleUI();
        
        AssignUniqueSprites();
    }
}
