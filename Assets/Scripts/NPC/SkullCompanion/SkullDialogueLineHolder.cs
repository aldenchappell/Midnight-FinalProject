using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullDialogueLineHolder : MonoBehaviour
{
    public static SkullDialogueLineHolder Instance;
    public static SkullDialogue SkullDialogue;
    
    public bool isFirstClipOfScene = true;
    public AudioSource audioSource;
    
    public AudioClip[] levelCompletedClips;
    public AudioClip[] wittyAssRemarks;
    public AudioClip[] demonChasingClips;
    public AudioClip demonPatrollingClip;
    public AudioClip demonRoamingClip;
    
    [Space(10)]
    public AudioClip lobbyOpeningClip;
    public AudioClip findFuzeClip;
    
    [Space(10)]
    public AudioClip floorOneOpeningClip;
    public AudioClip findBookClip;
    public AudioClip returnBookClip;
    public AudioClip findPerfumeBottleClip;
    public AudioClip returnPerfumeBottleClip;
    
    [Space(10)]
    public AudioClip floorTwoOpeningClip;
    public AudioClip solveBabyBlockPuzzleClip;
    public AudioClip solveDaVinciPuzzleClip;
    public AudioClip solveBabyBlockAndDaVinciPuzzlesClip;
    public AudioClip solveMusicBoxPuzzleClip;
    
    [Space(10)]
    public AudioClip floorThreeOpeningClip;
    public AudioClip solveImagePuzzleClip;
    public AudioClip solveMazeBallPuzzleClip;
    public AudioClip solveImageAndMazeBallPuzzlesClip;
    public AudioClip solvePolaroidPuzzleClip;
    public AudioClip collectPolaroidsClip;
    
    
    private const float AudioCooldownTime = 5f; // Adjust this value as needed
    private float _lastAudioPlayTime = -Mathf.Infinity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SkullDialogue = FindObjectOfType<SkullDialogue>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator StartDialogueAnimation(float speakingTime)
    {
        SkullDialogue.skullAnimator.SetBool("Speaking", true);
        Debug.Log(SkullDialogue.skullAnimator.GetBool("Speaking"));
        yield return new WaitForSeconds(speakingTime);
        SkullDialogue.skullAnimator.SetBool("Speaking", false);
    }
    
    public bool CanPlayAudio()
    {
        return Time.time >= _lastAudioPlayTime + AudioCooldownTime;
    }

    public void RecordAudioPlayTime()
    {
        _lastAudioPlayTime = Time.time;
    }

    public bool IsAudioSourcePlaying() => audioSource.isPlaying;

    public void PlaySpecificClip(AudioClip clip)
    {
        if (CanPlayAudio() && !IsAudioSourcePlaying())
        {
            audioSource.PlayOneShot(clip);
            StartCoroutine(StartDialogueAnimation(clip.length - .3f));
            RecordAudioPlayTime();
        }
    }

    public void PlayRandomClip(AudioClip[] clips)
    {
        if (CanPlayAudio() && clips.Length > 0 && !IsAudioSourcePlaying())
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip selectedClip = clips[randomIndex];
        
            audioSource.PlayOneShot(selectedClip);
            StartCoroutine(StartDialogueAnimation(selectedClip.length - .3f));
            RecordAudioPlayTime();
        }
    }



    public int GetRandomWaitTime()
    {
        return Random.Range(15,45);
    }

    public AudioClip GetHintClipForRemainingPuzzles(List<string> remainingPuzzles)
    {
        if (remainingPuzzles.Contains("LobbyPuzzle")) return findFuzeClip;
        if (remainingPuzzles.Contains("Bookshelf Puzzle")) return findBookClip;
        if (remainingPuzzles.Contains("Perfume Bottle Puzzle")) return findPerfumeBottleClip;
        if (remainingPuzzles.Contains("Sliding Image Puzzle")) return solveImagePuzzleClip;
        if (remainingPuzzles.Contains("Baby Block Puzzle")) return solveBabyBlockPuzzleClip;
        if (remainingPuzzles.Contains("Da Vinci Puzzle")) return solveDaVinciPuzzleClip;
        if (remainingPuzzles.Contains("Music Box Puzzle")) return solveMusicBoxPuzzleClip;
        if (remainingPuzzles.Contains("Maze Ball Puzzle")) return solveMazeBallPuzzleClip;
        if (remainingPuzzles.Contains("Polaroid Puzzle")) return solvePolaroidPuzzleClip;
        return null;
    }
}
