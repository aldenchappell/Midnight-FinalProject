using System;
using UnityEngine;
using System.Collections.Generic;

public class LevelCompletionManager : MonoBehaviour
{
    public static LevelCompletionManager Instance;
    public List<string> currentLevelPuzzles = new List<string>();
    public List<string> loadedLevels = new List<string>();
    private string _currentLevel;
    
    private readonly Dictionary<string, bool> _skullDialoguePlayed = new Dictionary<string, bool>();

    // Assign the appropriate puzzle scriptable objects in the inspector - Should be added to the lobby scene.
    public List<SO_Puzzle> lobbyPuzzles;
    public List<SO_Puzzle> level1Puzzles;
    public List<SO_Puzzle> level2Puzzles;
    public List<SO_Puzzle> level3Puzzles;

    private int _collectedKeys;
    private HashSet<string> _completedLevels = new HashSet<string>();
    private HashSet<string> _completedPuzzles = new HashSet<string>();

    private AudioSource _audioSource;
    [SerializeField] private AudioClip keyDropSound;
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
            if (currentLevelPuzzles.Count == 0)
            {
                SaveLevelCompletion(_currentLevel);
            }
        }
    }

    private void SaveLevelCompletion(string levelName)
    {
        _completedLevels.Add(levelName);
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

    public void StartLevel(string levelName, List<SO_Puzzle> puzzles)
    {
        _currentLevel = levelName;
        currentLevelPuzzles = ConvertPuzzlesToNames(puzzles);

        foreach (var puzzleName in currentLevelPuzzles)
        {
            if (!_completedPuzzles.Contains(puzzleName))
            {
                //Debug.Log("Incomplete puzzle: " + puzzleName);
            }
        }
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

    
    public bool HasCurrentLevelAlreadyBeenLoaded(string levelName)
    {
        //Debug.Log("HasSkullDialogueBeenPlayed: Level " 
                 // + levelName + " played status: " 
                //  + (_skullDialoguePlayed.ContainsKey(levelName) && _skullDialoguePlayed[levelName]));
        return loadedLevels.Contains(levelName);
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
        if (!IsLevelCompleted("LOBBY"))
        {
            StartLevel("LOBBY", lobbyPuzzles);
        }
        else if (!IsLevelCompleted("FLOOR ONE"))
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
            //Debug.Log("All levels completed!");
            // Game is won, handle that - load credits and/or ending cutscene?
            ResetPuzzles();
        }
    }
    
    public bool HasSkullDialogueBeenPlayed(string levelName)
    {
        return _skullDialoguePlayed.ContainsKey(levelName) && _skullDialoguePlayed[levelName];
    }

    public void SetSkullDialoguePlayed(string levelName)
    {
        _skullDialoguePlayed[levelName] = true;
        //Debug.Log("SetSkullDialoguePlayed: Marked dialogue as played for " + levelName);
    }

    public void OnPlayerDeath()
    {
        ResetPuzzles();
    }

    public void UpdateKeyCount(int keys)
    {
        _collectedKeys = keys;
    }
    
    public void OnKeySpawn()
    {
        _audioSource.PlayOneShot(keyDropSound);
        Debug.Log("Dropping key");
    }
    
    public int GetCollectedKeys()
    {
        return _collectedKeys;
    }

    public void CollectKey()
    {
        _collectedKeys++;

        // Optionally, trigger other events or UI updates related to key collection
    }
}