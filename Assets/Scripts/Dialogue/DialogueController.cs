using System;
using System.Collections;
using StarterAssets;
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

    private const float NoAudioDialogueSpamPreventionTime = 1.0f;
    private const float WithAudioDialogueSpamPreventionTime = 1.5f;
    private bool _shouldPrintText = true;
    private bool _isPrintingLine = false;
    private Coroutine _currentCoroutine = null;

    [SerializeField] private SkullDialogue skullCompanion;
    private PlayerDualHandInventory _playerInventory;
    private FirstPersonController _firstPersonController;
    
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
        
        //_skullCompanion = GameObject.FindWithTag("Skull").GetComponent<SkullDialogue>();
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>().GetComponent<PlayerDualHandInventory>();
        _firstPersonController = FindObjectOfType<FirstPersonController>().GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        ResetDialogueText();
        DisableDialogueBox();
    }

    private void Update()
    {
        if (_firstPersonController.canMove && _firstPersonController.canRotate)
        {
            dialogueBox.SetActive(false);
        }
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("Dialogue lines are null or empty!");
            return;
        }

        if (!_playerInventory.IsSkullInFirstSlot())
        {
            dialogueText.text = "";
            _lines = lines;
            _audioClips = null;
            _currentIndex = 0;
            EnableDialogueBox();
            _currentCoroutine = StartCoroutine(ReadOutLine());
        }
    }


    public void GoToNextLine()
    {
        if (_shouldPrintText && !_isPrintingLine)
        {
            _currentIndex++;

            if (_currentIndex < _lines.Length)
            {
                ResetDialogueText();

                if (_currentCoroutine != null)
                {
                    StopCoroutine(_currentCoroutine);
                }

                if (_audioClips != null && _audioClips.Length > 0)
                {
                    _currentCoroutine = StartCoroutine(ReadOutLineWithAudio());
                }
                else
                {
                    _currentCoroutine = StartCoroutine(ReadOutLine());
                }
            }
            else
            {
                StopDialogue();
            }
        }
        else if (_isPrintingLine)
        {
            StopCoroutine(_currentCoroutine);
            dialogueText.text = _lines[_currentIndex];
            _isPrintingLine = false;
            if (_audioClips != null && _audioClips.Length > 0)
            {
                StartCoroutine(PreventTextWithAudioDialogueSpam());
            }
            else
            {
                StartCoroutine(PreventTextOnlyDialogueSpam());
            }
        }
        //Debug.Log(_currentIndex);
    }

    private IEnumerator ReadOutLine()
    {
        _isPrintingLine = true;
        dialogueText.text = "";
        foreach (char character in _lines[_currentIndex])
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(textReadingSpeed);
        }
        _isPrintingLine = false;
        StartCoroutine(PreventTextOnlyDialogueSpam());
    }

    private IEnumerator ReadOutLineWithAudio()
    {
        _isPrintingLine = true;
        dialogueText.text = "";

        if (_audioClips != null && _audioClips.Length > 0)
        {
            audioSource.clip = _audioClips[_currentIndex % _audioClips.Length];
            audioSource.Play();
        }

        foreach (char character in _lines[_currentIndex])
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(textReadingSpeed);
        }

        if (_audioClips != null && _audioClips.Length > 0)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        _isPrintingLine = false;
        StartCoroutine(PreventTextWithAudioDialogueSpam());
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

        //if (GlobalCursorManager.Instance != null)
            GlobalCursorManager.Instance.EnableCursor();
    }

    public void DisableDialogueBox()
    {
        dialogueEnabled = false;
        ResetDialogueText();
        dialogueBox.SetActive(false);
        _shouldPrintText = true;
        _lines = null;
        _audioClips = null;
        audioSource.clip = null;
        dialogueText.text = "";

        if (GlobalCursorManager.Instance != null)
            GlobalCursorManager.Instance.DisableCursor();
    }

    private void StopDialogue()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        ResetDialogueText();
        DisableDialogueBox();

        if (audioSource.clip != null)
            audioSource.clip = null;
    }

    private IEnumerator PreventTextOnlyDialogueSpam()
    {
        _shouldPrintText = false;
        yield return new WaitForSeconds(NoAudioDialogueSpamPreventionTime);
        _shouldPrintText = true;
    }

    private IEnumerator PreventTextWithAudioDialogueSpam()
    {
        _shouldPrintText = false;
        yield return new WaitForSeconds(WithAudioDialogueSpamPreventionTime);
        _shouldPrintText = true;
    }
}