using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour, IPlaySkullDialogue
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
                PlaySpecificSkullDialogueClip(
                    SkullDialogueLineHolder.Instance.audioSource,
                    puzzleCompletionAudioClip);
            }
            
            onPuzzleCompletion?.Invoke();
        }
    }

    public void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            source.PlayOneShot(clip);
        }
    }

    public void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }

    public void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip)
    {
        if (!SkullDialogueLineHolder.Instance.IsAudioSourcePlaying())
        {
            
        }
    }

    public IEnumerator PlaySkullDialoguePuzzleHintClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip)
    {
        yield return null;
    }
}