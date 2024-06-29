using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using TMPro;
using Random = UnityEngine.Random;

public class SlidingImagePuzzle : MonoBehaviour, IPlaySkullDialogue
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

    [Header("Cinemachine Cameras")]
    private CinemachineVirtualCamera _mainCam;
    private CinemachineVirtualCamera _puzzleCam;
    
    
    [Header("Audio")]
    private AudioSource _audio;
    [SerializeField] private AudioClip correctMoveSound;
    [SerializeField] private AudioClip puzzleCompletedSound;
    
    private PuzzleEscape _puzzleEscape;
    private GameObject _playerUI;

    private bool _solved;
    private PatrolSystemManager _patrol;

    [SerializeField] private GameObject interactableMazeBall;
    private void Awake()
    {
        _puzzle = GetComponent<Puzzle>();
        _audio = GetComponent<AudioSource>();
        _puzzleEscape = GetComponent<PuzzleEscape>();
        _playerUI = GameObject.Find("PlayerUICanvas");
        _mainCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("SlidingImagePuzzleCam").GetComponent<CinemachineVirtualCamera>();
        _patrol = GameObject.Find("DemonPatrolManager").GetComponent<PatrolSystemManager>();
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
        if (_solved)
        {
            _audio.PlayOneShot(puzzleCompletedSound);
            return;
        }
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        
        firstPersonController.canMove = !puzzleUI.activeSelf;
        firstPersonController.controller.enabled = !puzzleUI.activeSelf;
        if (puzzleUI.activeSelf)
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

        //assign the random sprites
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
        else if (!IsAdjacentSlot(_selectedSlotIndex, clickedIndex))
        {
            _audio.PlayOneShot(puzzleCompletedSound);
        }
        else
        {
            _audio.PlayOneShot(puzzleCompletedSound);
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
            TogglePuzzleUI();
            _solved = true;
            _puzzle.CompletePuzzle();
            firstPersonController.canMove = true;
            firstPersonController.canRotate = true;
        }
    }

    public void InstantiateMazeballAtPlayerFeet()
    {
        interactableMazeBall.SetActive(true);
    }

    private void UpdatePuzzleUI()
    {
        movesRemainingText.text = "Moves Made: " + _movesMade + "/" + _maxMoves;
    }

    private void ResetPuzzle()
    {
        _movesMade = 0;
        UpdatePuzzleUI();
        _patrol.DecreaseTimeToSpawn = 10;
        AssignUniqueSprites();
    }

    public void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying() && SkullDialogueLineHolder.SkullDialogue.pickedUp)
            source.PlayOneShot(clip);
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips)
    {
        
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        
    }

    public IEnumerator PlaySkullDialoguePuzzleHintClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying() && SkullDialogueLineHolder.SkullDialogue.pickedUp)
        {
            if (indexOfCurrentLevelPuzzles == 3)
            {
                while (true)
                {
                    if (source.isPlaying) yield return null;

                    yield return new WaitForSeconds(SkullDialogueLineHolder.Instance.GetRandomWaitTIme());
                    source.PlayOneShot(clip);
                    yield return new WaitForSeconds(SkullDialogueLineHolder.Instance.GetRandomWaitTIme());
                }
            }
            yield return null;
        }
    }
}