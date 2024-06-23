using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour, IPlaySkullDialogue
{
    public SO_Puzzle puzzle;
    public UnityEvent onPuzzleCompletion;
    private void Awake()
    {
        if (puzzle == null)
        {
            //Debug.LogError("Puzzle scriptable object is not assigned!");
        }
    }

    public void CompletePuzzle()
    {
        if (LevelCompletionManager.Instance != null)
        {
            LevelCompletionManager.Instance.SavePuzzleCompletion(puzzle);
            LevelCompletionManager.Instance.CompletePuzzleInScene(SceneManager.GetActiveScene().name, puzzle.puzzleName);
            
            PlayRandomSkullDialogueClip(
                SkullDialogueLineHolder.Instance.audioSource,
                SkullDialogueLineHolder.Instance.levelCompletedClips);
            onPuzzleCompletion?.Invoke();
        }
    }

    public void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip)
    {
        if(!source.isPlaying)
            source.PlayOneShot(clip);
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clip)
    {
        if(!source.isPlaying)
            source.PlayOneShot(clip[Random.Range(0, clip.Length)]);
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        
    }
}