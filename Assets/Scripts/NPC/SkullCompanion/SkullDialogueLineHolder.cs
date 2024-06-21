using System;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkullDialogueLineHolder : MonoBehaviour
{
    public AudioSource audioSource;

    [ColoredHeader("Universal Audio Clips", "#FF0000")]
    public AudioClip[] levelCompletedClips;
    public AudioClip[] wittyAssRemarks;
    public AudioClip demonChasingClip;
    public AudioClip demonPatrollingClip;
    public AudioClip demonRoamingClip;
    
    [Space(10)]
    
    [ColoredHeader("Lobby Audio Clips", "#00FF00")] 
    public AudioClip lobbyOpeningClip;
    public AudioClip findFuzeClip;
    
    [Space(10)]
    
    [ColoredHeader("Floor One Audio Clips", "#0000FF")] 
    public AudioClip floorOneOpeningClip;
    public AudioClip findBookClip;
    public AudioClip returnBookClip;
    public AudioClip findPerfumeBottleClip;
    public AudioClip returnPerfumeBottleClip;
    
    [Space(10)]
    
    [ColoredHeader("Floor Two Audio Clips", "#FFFF00")] 
    public AudioClip floorTwoOpeningClip;
    public AudioClip solveBabyBlockPuzzleClip;
    public AudioClip solveDaVinciPuzzleClip;
    public AudioClip solveBabyBlockAndDaVinciPuzzlesClip;
    public AudioClip solveMusicBoxPuzzleClip;
    
    [Space(10)]
    
    [ColoredHeader("Floor Three Audio Clips", "#FF00FF")] 
    public AudioClip floorThreeOpeningClip;
    public AudioClip solveImagePuzzleClip;
    public AudioClip solveMazeBallPuzzleClip;
    public AudioClip solveImageAndMazeBallPuzzlesClip;
    public AudioClip solvePolaroidPuzzleClip;
    public AudioClip collectPolaroidsClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlaySpecificSkullDialogueClip(AudioClip clip) => audioSource.PlayOneShot(clip);
    public void PlayRandomSkullDialogueClip(AudioClip[] clips) =>
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);

    public void PlaySkullDialogueClipWithLogic(bool value, AudioClip clip)
    {
        if (value)
        {
            PlaySpecificSkullDialogueClip(clip);
        }
    }
}