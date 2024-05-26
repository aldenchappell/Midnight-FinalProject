using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float textReadingSpeed = 0.05f;

    private int _currentIndex;
    private int _lastUsedIndex;
    
    private string[] _lines;

    [SerializeField] private GameObject dialogueBox;

    [HideInInspector] public bool dialogueEnabled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetDialogueText();
        DisableDialogueBox();
    }

    public void StartDialogue(string[] lines)
    {
        _lines = lines;
        _currentIndex = 0;
        EnableDialogueBox();
        StartCoroutine(ReadOutLine());
        
    }

    public void GoToNextLine()
    {
        if (_currentIndex < _lines.Length - 1)
        {
            _currentIndex++;
            dialogueText.text = "";
            StartCoroutine(ReadOutLine());
        }
        else
        {
            StopDialogue();
        }
    }

    private IEnumerator ReadOutLine()
    {
        foreach (var character in _lines[_currentIndex].ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(textReadingSpeed);
        }
    }

    private void ResetDialogueText()
    {
        dialogueText.text = "";
    }

    private void EnableDialogueBox()
    {
        dialogueEnabled = true;
        dialogueBox.SetActive(true);
        GlobalCursorManager.Instance.EnableCursor();
    }

    public void DisableDialogueBox()
    {
        _lastUsedIndex = _currentIndex;
        dialogueEnabled = false;
        dialogueBox.SetActive(false);
        GlobalCursorManager.Instance.DisableCursor();
    }

    private void StopDialogue()
    {
        _currentIndex = _lastUsedIndex;
        gameObject.SetActive(false);
        StopAllCoroutines();
        ResetDialogueText();
        DisableDialogueBox();
    }
}