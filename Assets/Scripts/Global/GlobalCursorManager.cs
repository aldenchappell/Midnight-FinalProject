using UnityEngine;

public class GlobalCursorManager : MonoBehaviour
{
    public static GlobalCursorManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad((gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void EnableCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableCursor()
    {
       //print("Disabling cursor");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
