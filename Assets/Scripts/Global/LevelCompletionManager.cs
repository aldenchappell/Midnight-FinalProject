using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelCompletionManager : MonoBehaviour
{
    public static LevelCompletionManager Instance;
    public List<string> currentLevelPuzzles = new List<string>();
    public List<string> loadedLevels = new List<string>();
    public string _currentLevel;
    public bool hasCompletedLobby = false;
    
    private readonly Dictionary<string, bool> _skullDialoguePlayed = new Dictionary<string, bool>();

    // Assign the appropriate puzzle scriptable objects in the inspector - Should be added to the lobby scene.
    public List<SO_Puzzle> lobbyPuzzles;
    public List<SO_Puzzle> level1Puzzles;
    public List<SO_Puzzle> level2Puzzles;
    public List<SO_Puzzle> level3Puzzles;

    private int _collectedKeys;
    private int _collectedIdols;
    public bool allIdolsCollected;
    //Added this line for fix: Owen
    
    public int _keysReturned = 0;
    
    private HashSet<string> _completedLevels = new HashSet<string>();
    private HashSet<string> _completedPuzzles = new HashSet<string>();

    private AudioSource _audioSource;
    [SerializeField] private AudioClip keyDropSound;

    public bool hasKey = false;
    public bool allLevelsCompleted = false;
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
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void SavePuzzleCompletion(SO_Puzzle puzzle)
    {
        if (currentLevelPuzzles == null)
        {
            return;
        }

        _completedPuzzles.Add(puzzle.puzzleName);
        if (currentLevelPuzzles.Contains(puzzle.puzzleName))
        {
            currentLevelPuzzles.Remove(puzzle.puzzleName);
            
        }
    }

    public void SaveLevelCompletion(string levelName)
    {
        //print("Checking for level comp");
        if (currentLevelPuzzles.Count == 0)
        {
            _completedLevels.Add(levelName);
        }
    }
    
    
    public bool IsLevelCompleted(string levelName)
    {
        return _completedLevels.Contains(levelName);
    }
    
    
    public void ResetPuzzles()
    {
        foreach (string puzzleName in currentLevelPuzzles)
        {
            _completedPuzzles.Remove(puzzleName);
        }
        currentLevelPuzzles = new List<string>();
    }

    public void ResetGame(bool shouldResetCubby)
    {
        _collectedKeys = 0;
        hasKey = false;
        
        allLevelsCompleted = false;
        allIdolsCollected = false;
        
        ResetPuzzles();
        
        _completedLevels.Clear();
        _completedPuzzles.Clear();
        _skullDialoguePlayed.Clear();
        
        hasCompletedLobby = false;
        //_keysReturned = 0;

        if (shouldResetCubby)
        {
            FindObjectOfType<KeyCubbyController>().ResetCubby();
            _keysReturned = 0;
        }
    }
    
    public void FinishGame()
    {
        allLevelsCompleted = true;
        //PowerLobbyIfNeeded();
    }

    private void PowerLobbyIfNeeded()
    {
        if (allLevelsCompleted && !hasCompletedLobby)
        {
            hasCompletedLobby = true;
            
            if(SceneManager.GetActiveScene().name == "LOBBY")
                FindObjectOfType<FuseBox>().PowerLobby();
        }
    }

    public void StartLevel(string levelName, List<SO_Puzzle> puzzles)
    {
        _currentLevel = levelName;
        currentLevelPuzzles = ConvertPuzzlesToNames(puzzles);
    }

    public void SaveCurrentLevelAsLoaded(string levelName)
    {
        _currentLevel = levelName;

        if (!loadedLevels.Contains(levelName))
        {
            loadedLevels.Add(levelName);
           // Debug.Log("SaveCurrentLevelAsLoaded: Loaded level " + levelName);
        }
    }

    private List<string> ConvertPuzzlesToNames(List<SO_Puzzle> puzzles)
    {
        List<string> names = new List<string>();
        foreach (SO_Puzzle puzzle in puzzles)
        {
            names.Add(puzzle.puzzleName);
        }
        return names;
    }
    
    public void CompletePuzzleInScene(string sceneName, string puzzleName)
    {
        string levelName = sceneName.ToUpper();
        if (!_completedLevels.Contains(levelName))
        {
            //Debug.Log("Level " + levelName + " not found in completed levels");
            return;
        }

        if (_completedPuzzles.Contains(puzzleName))
        {
            //Debug.Log("Puzzle " + puzzleName + " in level + " + levelName + " is already completed.");
            return;
        }

        _completedPuzzles.Add(puzzleName);
        //Debug.Log("Puzzle " + puzzleName + " in level " + levelName + " completed.");
    }

    public void CheckAndStartNextLevel()
    {
        if (!IsLevelCompleted("FLOOR ONE"))
        {
            StartLevel("FLOOR ONE", level1Puzzles);
        }
        else if (!IsLevelCompleted("FLOOR TWO"))
        {
            StartLevel("FLOOR TWO", level2Puzzles);
        }
        else if (!IsLevelCompleted("FLOOR THREE"))
        {
            StartLevel("FLOOR THREE", level3Puzzles);
        }
        else
        {
            //Debug.Log("Game complete");
            FinishGame(); 
        }
        InGameSettingsManager.Instance.LoadSettings();
    }

    
    public bool HasSkullDialogueBeenPlayed(string levelName)
    {
        return _skullDialoguePlayed.ContainsKey(levelName) && _skullDialoguePlayed[levelName];
    }

    public void SetSkullDialoguePlayed(string levelName)
    {
        _skullDialoguePlayed[levelName] = true;
    }

    public void OnPlayerDeath()
    {
        //ResetGame();
        ResetPuzzles();
    }
    
    public void OnKeySpawn()
    {
        _audioSource.PlayOneShot(keyDropSound);
    }
    

    public int GetCollectedKeys()
    {
        return _collectedKeys;
    }

    public void SetCollectedKeys(int keys)
    {
        _collectedKeys = keys;
    }

    public void CollectKey()
    {
        _collectedKeys++;
        hasKey = true;
    }
    
    public void UpdateKeyCount(int keys)
    {
        _collectedKeys = keys;
    }
    
    public int GetCollectedIdols()
    {
        return _collectedIdols;
    }

    public void CollectIdol()
    {
        _collectedIdols++;
    }

    public void UnlockSecretRoom()
    {
        SecretBookshelf bookshelf = FindObjectOfType<SecretBookshelf>();
        
        if (bookshelf != null)
            bookshelf.interactable.onInteraction.AddListener(bookshelf.UnlockBookshelfAccess);
    }
    public void CompleteObjective(Objective objective)
    {
        objective.isCompleted = true;
        FindObjectOfType<ObjectiveController>().UpdateTaskList();
    }
}