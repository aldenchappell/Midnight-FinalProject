using System.Collections.Generic;
using UnityEngine;

public class SkullDialogueLineHolder : MonoBehaviour
{
    public static SkullDialogueLineHolder Instance;

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
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsAudioSourcePlaying() => audioSource.isPlaying;
    
    public int GetRandomWaitTIme()
    {
        return Random.Range(35, 60);
    }
    
    public AudioClip GetHintClipForRemainingPuzzles(List<string> remainingPuzzles)
    {
        if (remainingPuzzles.Contains("LobbyPuzzle"))
        {
            return findFuzeClip;
        }
        
        if (remainingPuzzles.Contains("Bookshelf Puzzle"))
        {
            return findBookClip;
        }

        if (remainingPuzzles.Contains("Perfume Bottle Puzzle"))
        {
            return findPerfumeBottleClip;
        }
        
        if (remainingPuzzles.Contains("Sliding Image Puzzle"))
        {
            return solveImagePuzzleClip;
        }
        
        if (remainingPuzzles.Contains("Baby Block  Puzzle"))
        {
            return solveBabyBlockPuzzleClip;
        }
        
        if (remainingPuzzles.Contains("Da Vinci Puzzle"))
        {
            return solveDaVinciPuzzleClip;
        }
        
        if (remainingPuzzles.Contains("Music Box Puzzle"))
        {
            return solveMusicBoxPuzzleClip;
        }
        
        if (remainingPuzzles.Contains("Maze Ball Puzzle"))
        {
            return solveMazeBallPuzzleClip;
        }
        
        if (remainingPuzzles.Contains("Polaroid Puzzle"))
        {
            return solvePolaroidPuzzleClip;
        }

        return null;
        // just in case if no specific puzzle is left
        //return wittyAssRemarks.Length > 0 ? wittyAssRemarks[Random.Range(0, wittyAssRemarks.Length)] : null;
    }
}