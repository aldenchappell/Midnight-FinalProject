using UnityEngine;

public class MazeBall : MonoBehaviour
{
    private MazeBullPuzzle _mazePuzzle;

    private void Awake()
    {
        _mazePuzzle = FindObjectOfType<MazeBullPuzzle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MazePuzzleCompletion")) return;

        _mazePuzzle.TogglePuzzleUI();
        _mazePuzzle.puzzle.CompletePuzzle();
    }
}
