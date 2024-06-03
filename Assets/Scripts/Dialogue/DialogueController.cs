using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float textReadingSpeed = 0.05f;
    [SerializeField] private GameObject dialogueBox;
    public AudioSource audioSource;

    private int _currentIndex;
    private string[] _lines;
    private AudioClip[] _audioClips;

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

    public void StartDialogueWithAudio(string[] lines, AudioClip[] audioClips)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("Dialogue lines are null or empty!");
            return;
        }
        
        _lines = lines;
        _audioClips = audioClips;
        _currentIndex = 0;
        EnableDialogueBox();
        StartCoroutine(ReadOutLineWithAudio());
    }

    public void GoToNextLine()
    {
        if (_shouldPrintText)
        {
            ResetDialogueText();
            StartCoroutine(PreventDialogueSpam());
            
            _currentIndex++;
            
            if (_currentIndex < _lines.Length)
            {
                dialogueText.text = "";
                StartCoroutine(ReadOutLine());
            }
            else
            {
                StopDialogue();
            }
        }
        Debug.Log(_currentIndex);
    }




    private IEnumerator ReadOutLine()
    {
        foreach (char character in _lines[_currentIndex].ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(textReadingSpeed);
        }
    }

    private IEnumerator ReadOutLineWithAudio()
    {
        if (_audioClips != null && _audioClips.Length > 0)
        {
            audioSource.clip = _audioClips[_currentIndex % _audioClips.Length];
            audioSource.Play();
        }

        foreach (char character in _lines[_currentIndex].ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(textReadingSpeed);
        }

        if (_audioClips != null && _audioClips.Length > 0)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
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

        if (GlobalCursorManager.Instance != null)
            GlobalCursorManager.Instance.EnableCursor();
    }

    public void DisableDialogueBox()
    {
        dialogueEnabled = false;
        ResetDialogueText();
        dialogueBox.SetActive(false);
        _shouldPrintText = true;

        if (GlobalCursorManager.Instance != null)
            GlobalCursorManager.Instance.DisableCursor();
    }

    private void StopDialogue()
    {
        StopAllCoroutines();
        ResetDialogueText();
        DisableDialogueBox();
        
        if (audioSource.clip != null)
            audioSource.clip = null;
    }

    private IEnumerator PreventDialogueSpam()
    {
        _shouldPrintText = false;
        yield return new WaitForSeconds(DialogueSpamPreventionTime);
        _shouldPrintText = true;
    }
}
