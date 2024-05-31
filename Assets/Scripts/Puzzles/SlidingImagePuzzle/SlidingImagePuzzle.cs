using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using StarterAssets;
using Random = UnityEngine.Random;

public class SlidingImagePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject puzzleUI;

    [SerializeField] private Sprite[] puzzleSprites;
    [SerializeField] private List<Image> gridSlotImages;

    private int _selectedSlotIndex = -1;
    private bool _isFirstSlotSelected;

    
    [SerializeField] private FirstPersonController firstPersonController; 
    private void Start()
    {
        AssignUniqueSprites();
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
        else
        {
            SwapSlotImages(_selectedSlotIndex, clickedIndex);
            _isFirstSlotSelected = false;
        }
    }

    private void SwapSlotImages(int index1, int index2)
    {
        //if an invalid index is selected
        if (index1 < 0 || index1 >= gridSlotImages.Count || index2 < 0 || index2 >= gridSlotImages.Count)
        {
            return;
        }

        //swap the grid slot images
        Image image1 = gridSlotImages[index1];
        Image image2 = gridSlotImages[index2];

        Sprite tempSprite = image1.sprite;
        image1.sprite = image2.sprite;
        image2.sprite = tempSprite;
    }

}
