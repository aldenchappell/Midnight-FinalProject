using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour
{
    public SO_Puzzle puzzle;
    public UnityEvent onPuzzleCompletion;
    [SerializeField] private AudioClip puzzleCompletionAudioClip;

    public void CompletePuzzle()
    {
        if (LevelCompletionManager.Instance != null)
        {
            LevelCompletionManager.Instance.SavePuzzleCompletion(puzzle);
            LevelCompletionManager.Instance.CompletePuzzleInScene(SceneManager.GetActiveScene().name, puzzle.puzzleName);

            if (puzzleCompletionAudioClip != null)
            {
                if (SkullDialogueLineHolder.Instance.CanPlayAudio())
                {
                    SkullDialogueLineHolder.Instance.audioSource.PlayOneShot(puzzleCompletionAudioClip);
                    SkullDialogueLineHolder.Instance.RecordAudioPlayTime();
                }
            }
            
            onPuzzleCompletion?.Invoke();
        }
    }
}