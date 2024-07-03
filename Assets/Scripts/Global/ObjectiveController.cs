using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveController : MonoBehaviour
{
    private Dictionary<Scene, string> sceneObjectives = new Dictionary<Scene, string>();

    private void Awake()
    {
        InitializeObjectives();    
    }

    private void InitializeObjectives()
    {
        
    }
    
    
    
    
    private string GetCurrentSceneName() => SceneManager.GetActiveScene().name;
}
