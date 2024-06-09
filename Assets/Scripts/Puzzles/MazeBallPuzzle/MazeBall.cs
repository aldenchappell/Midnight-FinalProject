using UnityEngine;

public class MazeBall : MonoBehaviour
{
    private MazeBallPuzzle _mazePuzzle;

    private void Awake()
    {
        _mazePuzzle = FindObjectOfType<MazeBallPuzzle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MazePuzzleCompletion")) return;

        _mazePuzzle.solved = true;
        _mazePuzzle.TogglePuzzleUI();
        _mazePuzzle.puzzle.CompletePuzzle();
        _mazePuzzle.Complete();
        _mazePuzzle.ResetRotation();
        
        Destroy(gameObject);
    }
}
