using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDoorExit : MonoBehaviour
{
    [SerializeField] private GameObject outroCutscene;
    [SerializeField] private Image textPanel;
    [SerializeField] private TMP_Text text;
    public GameObject particles;
    private Coroutine _textCoroutine;
    private KeyCubbyController _keyCubbyController;
    private FadeUI _fadeUI;
    private void Start()
    {
        _keyCubbyController = FindObjectOfType<KeyCubbyController>();
    }

    public void PlayOutroCutscene()
    {
        if (LevelCompletionManager.Instance.allLevelsCompleted)
        {
            outroCutscene.SetActive(true);
            LevelCompletionManager.Instance.ResetGame();
            _keyCubbyController.ResetCubby();
        }
        else
        {
            _fadeUI.FadeInAndOutText(text);
        }
    }
}
