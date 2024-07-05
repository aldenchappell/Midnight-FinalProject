using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelCompletionManager : MonoBehaviour
{
    public static LevelCompletionManager Instance;
    public List<string> currentLevelPuzzles = new List<string>();
    public List<string> loadedLevels = new List<string>();
    private string _currentLevel;
    public bool hasCompletedLobby = false;
    
    private readonly Dictionary<string, bool> _skullDialoguePlayed = new Dictionary<string, bool>();

    // Assign the appropriate puzzle scriptable objects in the inspector - Should be added to the lobby scene.
    public List<SO_Puzzle> lobbyPuzzles;
    public List<SO_Puzzle> level1Puzzles;
    public List<SO_Puzzle> level2Puzzles;
    public List<SO_Puzzle> level3Puzzles;

    private int _collectedKeys;
    public HashSet<string> completedLevels = new HashSet<string>();
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
    
    
    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            allLevelsCompleted = true;
        }
        #endif
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
            if (currentLevelPuzzles.Count == 0)
            {
                SaveLevelCompletion(_currentLevel);
            }
        }
    }

    private void SaveLevelCompletion(string levelName)
    {
        completedLevels.Add(levelName);
    }
    
    
    public bool IsLevelCompleted(string levelName)
    {
        return completedLevels.Contains(levelName);
    }
    
    
    public void ResetPuzzles()
    {
        foreach (string puzzleName in currentLevelPuzzles)
        {
            _completedPuzzles.Remove(puzzleName);
        }
        currentLevelPuzzles = new List<string>();
        
        //_completedPuzzles.Clear();
       // completedLevels.Clear();
       // currentLevelPuzzles.Clear();
        //loadedLevels.Clear();
    }

    public void ResetGame()
    {
        _collectedKeys = 0;
        allLevelsCompleted = false;
    }
    
    public void FinishGame()
    {
        allLevelsCompleted = true;
        // Handle other game finish actions if needed
        // Ensure to keep the lobby powered until the game is won
        PowerLobbyIfNeeded();
    }

    private void PowerLobbyIfNeeded()
    {
        if (allLevelsCompleted && !hasCompletedLobby)
        {
            hasCompletedLobby = true;
            PlayerPrefs.SetInt("LobbyPowered", 1);
            PlayerPrefs.Save();
            
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
        if (!completedLevels.Contains(levelName))
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
            //game completed
            //ResetGame();
            ResetPuzzles();
            ResetGame();
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
        //ResetPuzzles();
        //ResetGame();
    }
    
    public void OnKeySpawn()
    {
        //print("Key spawn Audio");
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
}