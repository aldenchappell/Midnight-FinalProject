using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    private PhotoBoardPuzzle _puzzle;
    [SerializeField] private Image photoBoardImage;

    public bool isPolaroidPrefab;
    private void Awake()
    {
        _puzzle = FindObjectOfType<PhotoBoardPuzzle>();
        if (isPolaroidPrefab)
        {
            photoBoardImage = GameObject.Find("PuzzlePiece07").GetComponent<Image>();
            
        }
    }

    public void IncrementPolaroidCount()
    {
        if (!isPolaroidPrefab)
        {
            _puzzle.polaroidCount++;
            Destroy(gameObject);
        }
    }

    public void SetPhotoBoardPieceAlpha()
    {
        photoBoardImage.color = new Color(photoBoardImage.color.r, photoBoardImage.color.g, photoBoardImage.color.b, 1.0f);
    }
}
