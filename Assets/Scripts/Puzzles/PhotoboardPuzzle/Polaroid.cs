using UnityEngine;

public class Polaroid : MonoBehaviour
{
    private PhotoBoardPuzzle _puzzle;
    
    private void Awake()
    {
        _puzzle = FindObjectOfType<PhotoBoardPuzzle>();
    }

    public void IncrementPolaroidCount()
    {
        _puzzle.polaroidCount++;
        
        Destroy(gameObject);
    }
}
