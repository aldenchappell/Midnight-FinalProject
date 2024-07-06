using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    private PhotoBoardPuzzle _puzzle;
    [SerializeField] private Image photoBoardImage;

    public bool isPolaroidPrefab;
    [SerializeField] private TMP_Text polaroidCountText;
    [SerializeField] private Image polaroidCountImage;
    private void Awake()
    {
        _puzzle = FindObjectOfType<PhotoBoardPuzzle>();
        if (isPolaroidPrefab)
        {
            photoBoardImage = GameObject.Find("PuzzlePiece07")?.GetComponent<Image>();
        }
        
        FadeUI fadeUI = FindObjectOfType<FadeUI>();
        if (polaroidCountText != null && polaroidCountImage != null)
        {
            fadeUI.fadeDuration = 3.0f;
            fadeUI.FadeOutText(polaroidCountText);
            fadeUI.FadeOutImage(polaroidCountImage);
        }
    }

    private void Start()
    {
        if(!isPolaroidPrefab)
            polaroidCountText.text =  _puzzle.polaroidCount.ToString();
    }

    public void IncrementPolaroidCount()
    {
        _puzzle.polaroidCount++;
        
        FadeUI fadeUI = FindObjectOfType<FadeUI>();
        
        if (polaroidCountText != null && polaroidCountImage != null)
        {
            fadeUI.fadeDuration = 3.0f;
            fadeUI.FadeInAndOutText(polaroidCountText);
            fadeUI.FadeInAndOutImage(polaroidCountImage);
        }
        
        polaroidCountText.text = _puzzle.polaroidCount.ToString();
        if (_puzzle.polaroidCount == 6)
        {
            GameObject polaroidPuzzleObjective = GameObject.Find("PuzzleObjective");
            polaroidPuzzleObjective.GetComponent<Objective>().CompleteObjective();
        }
        
        Destroy(gameObject);
    }

    public void SetPhotoBoardPieceAlpha()
    {
        photoBoardImage.color = new Color(photoBoardImage.color.r, photoBoardImage.color.g, photoBoardImage.color.b, 1.0f);
    }
}