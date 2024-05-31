using UnityEngine;

public class MazeBall : MonoBehaviour
{
    [SerializeField] private MazeBullPuzzle mazePuzzle;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MazePuzzleCompletion")) return;

        mazePuzzle.puzzle.CompletePuzzle();
    }
}
