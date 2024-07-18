using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float textReadingSpeed = 0.05f;
    public GameObject dialogueBox;
    public AudioSource audioSource;

    private int _currentIndex;
    private string[] _lines;
    [SerializeField] private AudioClip[] whisperSoundClips;
    
    
    [HideInInspector] public bool dialogueEnabled;

    private const float NoAudioDialogueSpamPreventionTime = 1.0f;
    private const float WithAudioDialogueSpamPreventionTime = 1.5f;
    private bool _shouldPrintText = true;
    private bool _isPrintingLine = false;
    private Coroutine _currentCoroutine = null;
    private GameObject _currentNPC;

    private PlayerDualHandInventory _playerInventory;
    private FirstPersonController _firstPersonController;

    private GameObject _currentDialogueNPC;
    private bool _inDialogue = false;
    
    
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

        _playerInventory = FindObjectOfType<PlayerDualHandInventory>();
        _firstPersonController = FindObjectOfType<FirstPersonController>();
    }

    private void Start()
    {
        ResetDialogueText();
        DisableDialogueBox();
    }

    private void Update()
    {
        if(_currentDialogueNPC != null)
        {
            if (Vector3.Distance(_firstPersonController.transform.position, _currentDialogueNPC.transform.position) > 5f)
            {
                print("Outside Distance");
                _currentDialogueNPC = null;
                StopDialogue(_currentNPC);
            }
        }
        if(_currentDialogueNPC != null)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                GoToNextLine();
            }
            if(Input.GetMouseButtonDown(1))
            {
                StopDialogue(_currentNPC);
            }
        }
        
    }

    public void StartDialogue(string[] lines, GameObject npc)
    {
        npc.GetComponent<Collider>().enabled = false;
        _currentNPC = npc;
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("Dialogue lines are null or empty!");
            return;
        }
        
        if(_inDialogue) return;

        _inDialogue = true;
        dialogueText.text = "";
        
        if (!_playerInventory.IsSkullInFirstSlot())
        {
            dialogueText.text = "";
            _lines = lines;
            
            _currentIndex = 0;
            EnableDialogueBox();
            _currentCoroutine = StartCoroutine(ReadOutLine());
            _currentDialogueNPC = npc;
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
                
                //reset the whisper sound effect when going to the next line
                audioSource.Stop();
                audioSource.clip = null;
                
                
                _currentCoroutine = StartCoroutine(ReadOutLine());
            }
            else
            {
                StopDialogue(_currentNPC);
            }
        }
        else if (_isPrintingLine)
        {
            StopCoroutine(_currentCoroutine);
            dialogueText.text = _lines[_currentIndex];
            _isPrintingLine = false;

            StartCoroutine(PreventTextOnlyDialogueSpam());
        }
    }

    private IEnumerator ReadOutLine()
    {
        _isPrintingLine = true;
        dialogueText.text = "";
        
        //play whisper sound when npc is reading out lines
        audioSource.clip = GetRandomAudioClip(whisperSoundClips);
        audioSource.Play();
        
        foreach (char character in _lines[_currentIndex])
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(textReadingSpeed);
        }
        _isPrintingLine = false;
        StartCoroutine(PreventTextOnlyDialogueSpam());
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
        GlobalCursorManager cursorManager = FindObjectOfType<GlobalCursorManager>();
        cursorManager.EnableCursor();
        _firstPersonController.canRotate = false;
    }

    public void DisableDialogueBox()
    {
        dialogueEnabled = false;
        ResetDialogueText();
        dialogueBox.SetActive(false);
        _shouldPrintText = true;
        _lines = null;
        
        audioSource.clip = null;
        GlobalCursorManager cursorManager = FindObjectOfType<GlobalCursorManager>();
        cursorManager.DisableCursor();
        
        _firstPersonController.canRotate = true;
    }

    private void StopDialogue(GameObject npc)
    {

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        ResetDialogueText();
        DisableDialogueBox();

        _inDialogue = false;
        
        if (audioSource.clip != null)
            audioSource.clip = null;
        npc.GetComponent<Collider>().enabled = true;
    }

    public void ResetDialogue()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        dialogueText.text = "";
        _lines = null;
        
        _currentIndex = 0;
        _isPrintingLine = false;
    }

    private IEnumerator PreventTextOnlyDialogueSpam()
    {
        _shouldPrintText = false;
        yield return new WaitForSeconds(NoAudioDialogueSpamPreventionTime);
        _shouldPrintText = true;
    }

    private AudioClip GetRandomAudioClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}