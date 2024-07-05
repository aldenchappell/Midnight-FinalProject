using UnityEngine;

public class CheckFirstLaunch : MonoBehaviour
{
    private void Start()
    {
        CheckForFirstLaunch();
    }
    
    private void CheckForFirstLaunch()
    {
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            Debug.Log("First Launch");
            PlayerPrefs.DeleteAll(); 
            PlayerPrefs.SetInt("FirstLaunch", 1); 
            PlayerPrefs.Save(); 
        }
    }
}
