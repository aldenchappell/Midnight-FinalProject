using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//written by Creature Sari
public class UIMenuButtons : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestSceneButton()
    {
        SceneManager.LoadScene("TEST SCENE");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
