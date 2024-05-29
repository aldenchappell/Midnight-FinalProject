using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class WoodBlockPuzzle : MonoBehaviour
{
    [SerializeField] private GameObject puzzleUI;
    [SerializeField] private Image[] slotImages;
    [SerializeField] private WoodBlockPuzzlePiece[] puzzlePieces;

    private int[] _correctSequence = new int[] { 10, 20, 30, 40 };
    private bool solved = false;

    [SerializeField] private FirstPersonController firstPersonController;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                WoodBlockPuzzlePiece puzzlePiece = hit.collider.GetComponent<WoodBlockPuzzlePiece>();

                if (puzzlePiece != null && !solved)
                {
                    Debug.Log("Found puzzle piece");
                    // Move the puzzle piece to the clicked slot
                    MovePuzzlePiece(puzzlePiece);
                }
            }
        }
    }


    public void TogglePuzzleUI()
    {
        bool isPuzzleActive = !puzzleUI.activeSelf;

        puzzleUI.SetActive(isPuzzleActive);

        // Toggle canMove and cursor visibility based on puzzle activity
        if (isPuzzleActive)
        {
            // Puzzle is active, disable player movement and show cursor
            firstPersonController.canMove = false;
            firstPersonController.controller.enabled = false;
            GlobalCursorManager.Instance.EnableCursor();
        }
        else
        {
            // Puzzle is inactive, enable player movement and hide cursor
            firstPersonController.canMove = true;
            firstPersonController.controller.enabled = true;
            GlobalCursorManager.Instance.DisableCursor();
        }
    }


    private void MovePuzzlePiece(WoodBlockPuzzlePiece puzzlePiece)
    {
        // Find the empty slot to place the puzzle piece
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i].sprite == null)
            {
                // Place the puzzle piece in the empty slot
                puzzlePiece.transform.position = slotImages[i].transform.position;
                slotImages[i].sprite = puzzlePiece.GetComponent<Image>().sprite;
                puzzlePiece.gameObject.SetActive(false); // Hide the puzzle piece

                CheckForCorrectSequence();
                return;
            }
        }
    }

    private void CheckForCorrectSequence()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i].sprite == null || puzzlePieces[i].puzzlePieceID != _correctSequence[i])
            {
                // If any slot is empty or puzzle piece is not in correct sequence, puzzle is not solved
                solved = false;
                return;
            }
        }

        // If all slots are filled and puzzle pieces are in correct sequence, puzzle is solved
        solved = true;
        Debug.Log("Puzzle Solved!");
        TogglePuzzleUI();
    }
}
