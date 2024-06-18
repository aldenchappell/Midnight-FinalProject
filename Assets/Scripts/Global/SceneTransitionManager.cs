using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static string PreviouslyLoadedSceneName { get; private set; } = "MAINMENU";

    private void Awake()
    {
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        UpdatePreviouslyLoadedScene("MAINMENU");
    }

    public static void UpdatePreviouslyLoadedScene(string sceneName)
    {
        PreviouslyLoadedSceneName = sceneName;
    }
}
