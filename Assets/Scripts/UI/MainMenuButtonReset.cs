using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonReset : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => LevelCompletionManager.Instance.ResetGame(false));
    }
}
