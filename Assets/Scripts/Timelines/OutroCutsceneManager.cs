using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class OutroCutsceneManager : MonoBehaviour
{
    public float changeTime;
    //public string sceneName;
    public PlayableDirector timeline;  // Reference to the PlayableDirector component
    public GameObject player;
    public string sceneName;

    // Update is called once per frame
    void Update()
    {
        // Check if the space bar is pressed or if the changeTime has elapsed
        if (Input.GetKeyDown(KeyCode.Space) || changeTime <= 0)
        {
            //Load the specified scene
            //SceneManager.LoadScene(sceneName);
            Loader.Load(Loader.Scene.MAINMENU);
            // Deactivate the timeline


            //timeline.Stop();
            // Optionally, deactivate the GameObject that holds the timeline
            //timeline.gameObject.SetActive(false);
            //player.SetActive(true);

            // Optionally, deactivate this GameObject
            // gameObject.SetActive(false);
        }

        // Decrease the changeTime
        changeTime -= Time.deltaTime;
    }
}
