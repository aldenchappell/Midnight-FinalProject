using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float textReadingSpeed = 0.05f;

    private int _currentIndex;
    private string[] _lines;

    [SerializeField] private GameObject dialogueBox;

    [HideInInspector] public bool dialogueEnabled;

    private const float DialogueSpamPreventionTime = 1.0f;
    private bool _shouldPrintText = true;
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
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("Dialogue lines are null or empty!");
            return;
        }
        _lines = lines;
        _currentIndex = 0;
        EnableDialogueBox();
        StartCoroutine(ReadOutLine());
    }

    public void GoToNextLine()
    {
        if (_shouldPrintText)
        {
            ResetDialogueText();
            StartCoroutine(PreventDialogueSpam());
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
    }

    private IEnumerator ReadOutLine()
    {
        foreach (char character in _lines[_currentIndex].ToCharArray())
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
        ResetDialogueText();
        dialogueEnabled = true;
        dialogueBox.SetActive(true);
        GlobalCursorManager.Instance.EnableCursor();
    }

    public void DisableDialogueBox()
    {
        dialogueEnabled = false;
        ResetDialogueText();
        dialogueBox.SetActive(false);
        _shouldPrintText = true;
        
        GlobalCursorManager.Instance.DisableCursor();
    }

    private void StopDialogue()
    {
        //gameObject.SetActive(false);
        StopAllCoroutines();
        ResetDialogueText();
        DisableDialogueBox();
    }

    private IEnumerator PreventDialogueSpam()
    {
        _shouldPrintText = false;
        yield return new WaitForSeconds(DialogueSpamPreventionTime);
        _shouldPrintText = true;
    }
}
