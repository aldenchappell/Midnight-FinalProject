using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    private PhotoBoardPuzzle _puzzle;
    [SerializeField] private Image photoBoardImage;

    public bool isPolaroidPrefab;
    [SerializeField] private TMP_Text polaroidCountText;
    private void Awake()
    {
        _puzzle = FindObjectOfType<PhotoBoardPuzzle>();
        if (isPolaroidPrefab)
        {
            photoBoardImage = GameObject.Find("PuzzlePiece07")?.GetComponent<Image>();
        }
    }

    private void Start()
    {
        if(!isPolaroidPrefab)
            polaroidCountText.text = "Polaroids collected: " + _puzzle.polaroidCount;
    }

    public void IncrementPolaroidCount()
    {
        if (!isPolaroidPrefab)
        {
            _puzzle.polaroidCount++;
            polaroidCountText.text = "Polaroids collected: " + _puzzle.polaroidCount;
            if (_puzzle.polaroidCount == 6)
            {
                _puzzle.GetComponentInChildren<Objective>().CompleteObjective();
            }
            Destroy(gameObject);
        }
    }

    public void SetPhotoBoardPieceAlpha()
    {
        photoBoardImage.color = new Color(photoBoardImage.color.r, photoBoardImage.color.g, photoBoardImage.color.b, 1.0f);
    }
}