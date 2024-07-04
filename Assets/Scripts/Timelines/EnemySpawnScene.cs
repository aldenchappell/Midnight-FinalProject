using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EnemySpawnScene : MonoBehaviour
{

    public float changeTime;
    //public string sceneName;
    public PlayableDirector timeline;  // Reference to the PlayableDirector component
    public GameObject player;

    private GameObject _camera;

    private void Awake()
    {
        _camera = GameObject.Find("MainCamera");
    }

    void Update()
    {
        // Check if the space bar is pressed or if the changeTime has elapsed
        if (Input.GetKeyDown(KeyCode.Space) || changeTime <= 0 || LevelCompletionManager.Instance.hasCompletedLobby == true)
        {
            // Load the specified scene
            //SceneManager.LoadScene(sceneName);

            // Deactivate the timeline


            timeline.Stop();
            player.SetActive(true);
            _camera.SetActive(true);
            // Optionally, deactivate the GameObject that holds the timeline
            timeline.gameObject.SetActive(false);


            // Optionally, deactivate this GameObject
            // gameObject.SetActive(false);
        }

        // Decrease the changeTime
        changeTime -= Time.deltaTime;
    }

}
