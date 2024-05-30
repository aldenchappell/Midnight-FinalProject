using UnityEngine;
using System.Collections.Generic;

public class LevelCompletionManager : MonoBehaviour
{
    public static LevelCompletionManager Instance;
    private List<string> _currentLevelPuzzles = new List<string>();
    private string _currentLevel;

    // Assign the appropriate puzzle scriptable objects in the inspector - Should be added to the lobby scene.
    public List<SO_Puzzle> level1Puzzles;
    public List<SO_Puzzle> level2Puzzles;
    public List<SO_Puzzle> level3Puzzles;
    public List<SO_Puzzle> aldenLevelPuzzles; //testing

    
    private HashSet<string> _completedLevels = new HashSet<string>();
    private HashSet<string> _completedPuzzles = new HashSet<string>();

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

    public void DestroyPuzzle(GameObject puzzleToDestroy)
    {
        Destroy(puzzleToDestroy);
    }

    public void SavePuzzleCompletion(SO_Puzzle puzzle)
    {
        if (_currentLevelPuzzles == null)
        {
            Debug.LogError("Current level puzzles list is null");
            return;
        }

        _completedPuzzles.Add(puzzle.puzzleName);
        if (_currentLevelPuzzles.Contains(puzzle.puzzleName))
        {
            _currentLevelPuzzles.Remove(puzzle.puzzleName);
            if (_currentLevelPuzzles.Count == 0)
            {
                SaveLevelCompletion(_currentLevel);

                // Debug log to show which puzzles are completed in the current level
                Debug.Log($"Completed puzzles in {_currentLevel}: {string.Join(", ", _completedPuzzles)}");
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
        foreach (string puzzleName in _currentLevelPuzzles)
        {
            _completedPuzzles.Remove(puzzleName);
        }
        _currentLevelPuzzles = new List<string>();
    }

    public void StartLevel(string levelName, List<SO_Puzzle> puzzles)
    {
        _currentLevel = levelName;
        _currentLevelPuzzles = ConvertPuzzlesToNames(puzzles);

        // Debug log to check if any puzzles are not completed in the current level
        Debug.Log("Starting level: " + _currentLevel);
        foreach (var puzzleName in _currentLevelPuzzles)
        {
            if (!_completedPuzzles.Contains(puzzleName))
            {
                Debug.Log("Incomplete puzzle: " + puzzleName);
            }
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
            Debug.Log("Level " + levelName + " not found in completed levels");
            return;
        }

        if (_completedPuzzles.Contains(puzzleName))
        {

            Debug.Log("Puzzle " + puzzleName + " in level + " + levelName + " is already completed.");
            return;
        }

        _completedPuzzles.Add(puzzleName);
        Debug.Log("Puzzle " + puzzleName + " in level " + levelName + "completed.");
    }

    public void CheckAndStartNextLevel()
    {
        if (!IsLevelCompleted("ALDEN"))
        {
            StartLevel("ALDEN", aldenLevelPuzzles);
        }
        else if (!IsLevelCompleted("LEVEL ONE"))
        {
            StartLevel("LEVEL ONE", level1Puzzles);
        }
        else if (!IsLevelCompleted("LEVEL TWO"))
        {
            StartLevel("LEVEL TWO", level2Puzzles);
        }
        else if (!IsLevelCompleted("LEVEL THREE"))
        {
            StartLevel("LEVEL THREE", level3Puzzles);
        }
        else
        {
            Debug.Log("All levels completed!");
            // Game is won, handle that - load credits and/or ending cutscene?
        }
    }

    public void OnPlayerDeath()
    {
        ResetPuzzles();
    }
}
