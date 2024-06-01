using UnityEngine;

public class MazeBall : MonoBehaviour
{
    public MazeBullPuzzle mazePuzzle;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MazePuzzleCompletion")) return;

        mazePuzzle.TogglePuzzleUI();
        mazePuzzle.puzzle.CompletePuzzle();
    }
}
