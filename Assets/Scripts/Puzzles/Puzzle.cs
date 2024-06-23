using System.Collections;
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

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips)
    {
        if(!source.isPlaying)
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        
    }

    public IEnumerator RepeatPlaySkullDialogueClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip)
    {
        yield return null;
    }
}