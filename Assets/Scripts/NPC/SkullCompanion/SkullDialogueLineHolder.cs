using UnityEngine;

public class SkullDialogueLineHolder : MonoBehaviour
{
    private AudioSource _audioSource;

    [ColoredHeader("Universal Audio Clips", "#FF0000")]
    public AudioClip demonChasingClip;
    public AudioClip demonPatrollingClip;
    public AudioClip demonRoamingClip;
    
    [Space(10)]
    
    [ColoredHeader("Lobby Audio Clips", "#00FF00")] 
    public AudioClip lobbyOpeningClip;
    
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
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlaySpecificAudioClip(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    public void PlayRandomAudioClip(AudioClip[] clips)
    {
        _audioSource.PlayOneShot(GetRandomAudioClip(clips));
    }
    
    private AudioClip GetRandomAudioClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}