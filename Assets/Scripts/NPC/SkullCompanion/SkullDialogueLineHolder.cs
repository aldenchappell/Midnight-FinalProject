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
        }
        else
        {
            Destroy(gameObject);
        }
    }
}