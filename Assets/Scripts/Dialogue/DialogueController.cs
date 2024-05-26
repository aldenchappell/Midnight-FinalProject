using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float textReadingSpeed;

    private int _index;
    private string[] _lines;

    [SerializeField] private GameObject dialogueBox;

    public bool dialogueEnabled = false;

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
        _index = 0;
        gameObject.SetActive(true);
        StartCoroutine(ReadOutLine());
        
        GlobalCursorManager.Instance.EnableCursor();
    }

    public void GoToNextLine()
    {
        if (_index < _lines.Length - 1)
        {
            _index++;
            dialogueText.text = "";
            StartCoroutine(ReadOutLine());
        }
        else
        {
            gameObject.SetActive(false);
            StopAllCoroutines();
            ResetDialogueText();
        }
    }

    private IEnumerator ReadOutLine()
    {
        foreach (var line in _lines[_index].ToCharArray())
        {
            dialogueText.text += line;
            yield return new WaitForSeconds(textReadingSpeed);
        }
    }

    private void ResetDialogueText()
    {
        dialogueText.text = "";
    }

    public void EnableDialogueBox()
    {
        dialogueEnabled = true;
        dialogueBox.SetActive(true);
        GlobalCursorManager.Instance.EnableCursor();
    }
    
    public void DisableDialogueBox()
    {
        dialogueEnabled = false;
        dialogueBox.SetActive(false);
        GlobalCursorManager.Instance.DisableCursor();
    }
}
