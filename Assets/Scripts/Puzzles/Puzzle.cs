using UnityEngine;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour
{
    public SO_Puzzle puzzle;

    private void Awake()
    {
        if (puzzle == null)
        {
            Debug.LogError("Puzzle scriptable object is not assigned!");
        }
    }

    public void CompletePuzzle()
    {
        if (LevelCompletionManager.Instance != null)
        {
            LevelCompletionManager.Instance.SavePuzzleCompletion(puzzle);
            LevelCompletionManager.Instance.CompletePuzzleInScene(SceneManager.GetActiveScene().name, puzzle.puzzleName);
            LevelCompletionManager.Instance.DestroyPuzzle(gameObject);
        }
        else
        {
            Debug.LogError("LevelCompletionManager instance is not available!");
        }
    }
}