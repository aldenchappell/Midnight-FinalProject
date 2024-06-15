using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{

    public float changeTime;
    public string sceneName;


    // Update is called once per frame
    void Update()
    {
        // Check if the space bar is pressed or if the changeTime has elapsed
        if (Input.GetKeyDown(KeyCode.Space) || changeTime <= 0)
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneName);
        }
    }
}
