using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{

    public float changeTime;
    public string sceneName;
    public PlayableDirector timeline;  // Reference to the PlayableDirector component

    // Update is called once per frame
    void Update()
    {
        // Check if the space bar is pressed or if the changeTime has elapsed
        if (Input.GetKeyDown(KeyCode.Space) || changeTime <= 0)
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneName);
            // Deactivate the timeline
            if (timeline != null)
            {
                timeline.Stop();
                // Optionally, deactivate the GameObject that holds the timeline
                timeline.gameObject.SetActive(false);
            }
            // Optionally, deactivate this GameObject
            // gameObject.SetActive(false);
        }
    }
}
