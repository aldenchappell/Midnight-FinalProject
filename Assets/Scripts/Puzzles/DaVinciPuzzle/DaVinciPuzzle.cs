using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;

public class DaVinciPuzzle : MonoBehaviour
{
    [Header("Letters in Da Vinci Code")]
    [SerializeField] string[] letters;
    [Header("Letters for passcode in order")]
    [SerializeField] int[] passcode;
    [Header("Da Vinci Puzzle UI")]
    [SerializeField] GameObject puzzleUI;
    [Header("Animator Child Object")]
    [SerializeField] Animator animatorChild;
    [Header("AudioFiles")]
    [SerializeField] AudioClip failSound;
    [SerializeField] AudioClip winSound;
    [Header("Dials")]
    [SerializeField] Transform[] dials;

    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _puzzleCam;

    private int _currentDialIndex;
    private bool _isActive;

    private GlobalCursorManager _cursor;
    private FirstPersonController _FPC;
    private AudioSource _puzzleAudio;
    private PatrolSystemManager _patrol;


    private int[] _dial1;
    private int[] _dial2;
    private int[] _dial3;
    private int[] _dial4;

    private bool _canAnimate;

    private void Awake()
    {
        GlobalCursorManager.Instance = _cursor;
        _currentDialIndex = 0;
        _FPC = GameObject.FindFirstObjectByType<FirstPersonController>();
        _puzzleAudio = GetComponent<AudioSource>();
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("DaVinciPuzzleCam").GetComponent<CinemachineVirtualCamera>();
        _patrol = GameObject.Find("DemonPatrolManager").GetComponent<PatrolSystemManager>();

        _canAnimate = true;
    }

    private void Start()
    {
        _dial1 = new int[1];
        _dial2 = new int[1];
        _dial3 = new int[1];
        _dial4 = new int[1];

        _dial1[0] = 0;
        _dial2[0] = 0;
        _dial3[0] = 0;
        _dial4[0] = 0;

        
    }

    private void Update()
    {
        if(_isActive)
        {
            CheckForInput();
        }
    }


    #region Adjusting Dials
    public void AdjustTargetDialLetter(int dialAdjustment, int dialIndex)
    {
        switch(dialIndex)
        {
            case 0:
                _dial1[0] += dialAdjustment;
                if(_dial1[0] > 25)
                {
                    _dial1[0] = 0;
                }
                if(_dial1[0] < 0)
                {
                    _dial1[0] = 25;
                }
                break;
            case 1:
                _dial2[0] += dialAdjustment;
                if (_dial2[0] > 25)
                {
                    _dial2[0] = 0;
                }
                if (_dial2[0] < 0)
                {
                    _dial2[0] = 25;
                }
                break;
            case 2:
                _dial3[0] += dialAdjustment;
                if (_dial3[0] > 25)
                {
                    _dial3[0] = 0;
                }
                if (_dial3[0] < 0)
                {
                    _dial3[0] = 25;
                }
                break;
            case 3:
                _dial4[0] += dialAdjustment;
                if (_dial4[0] > 25)
                {
                    _dial4[0] = 0;
                }
                if (_dial4[0] < 0)
                {
                    _dial4[0] = 25;
                }
                break;
        }
    }
 

    private void AdjustSelectedDial(int adjustment)
    {
        _currentDialIndex += adjustment;
        if(_currentDialIndex < 0)
        {
            _currentDialIndex = 3;
        }
        if(_currentDialIndex > 3)
        {
            _currentDialIndex = 0;
        }
        foreach(Transform dial in dials)
        {
            dial.GetChild(0).gameObject.SetActive(false);
        }
        dials[_currentDialIndex].GetChild(0).gameObject.SetActive(true);
    }

    private void CheckForInput()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            AdjustSelectedDial(-1);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            AdjustSelectedDial(1);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            AdjustTargetDialLetter(-1, _currentDialIndex);
            RotateDown(dials[_currentDialIndex].gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            AdjustTargetDialLetter(1, _currentDialIndex);
            RotateUp(dials[_currentDialIndex].gameObject);
        }
        //print(_dial1[0] + " " + _dial2[0] + " " + _dial3[0] + " " + _dial4[0]);
    }

    public void CheckForCorrectCombination()
    {
        int correctLetters = 0;
        if(_dial1[0] == passcode[0])
        {
            correctLetters++;
        }
        if (_dial2[0] == passcode[1])
        {
            correctLetters++;
        }
        if (_dial3[0] == passcode[2])
        {
            correctLetters++;
        }
        if (_dial4[0] == passcode[3])
        {
            correctLetters++;
        }
        if(correctLetters == 4)
        {
            if(_canAnimate)
            {
                print("Puzzle Solved");
                ActivatePuzzleUI();
                StartCoroutine(TriggerAnimation(false));
                GetComponent<Puzzle>().CompletePuzzle();
            }
        }
        else
        {
            if (_canAnimate)
            {
                print("Puzzle Failed");
                _patrol.DecreaseTimeToSpawn = 10;
                _patrol.ReferenceToSuspicion = transform.position;
                StartCoroutine(TriggerAnimation(true));
            }
        }
    }

    public void ActivatePuzzleUI()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        if(puzzleUI.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _FPC.ToggleCanMove();
            AdjustSelectedDial(0);
            _playerCam.Priority = 0;
            _puzzleCam.Priority = 5;
            _isActive = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _FPC.ToggleCanMove();
            foreach (Transform dial in dials)
            {
                dial.GetChild(0).gameObject.SetActive(false);
            }
            _playerCam.Priority = 5;
            _puzzleCam.Priority = 0;
            _isActive = false;
        }
        
    }
    #endregion

    #region Rotate Dials
    public void RotateUp(GameObject dial)
    {
        dial.transform.eulerAngles = new Vector3(dial.transform.eulerAngles.x, dial.transform.eulerAngles.y, dial.transform.eulerAngles.z - 13.84f);
    }

    public void RotateDown(GameObject dial)
    {
        dial.transform.eulerAngles = new Vector3(dial.transform.eulerAngles.x, dial.transform.eulerAngles.y, dial.transform.eulerAngles.z + 13.84f);
    }
    #endregion

    #region Animations
    private IEnumerator TriggerAnimation(bool fail)
    {
        _canAnimate = false;
        if(fail)
        {
            _puzzleAudio.PlayOneShot(failSound);
            MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer child in children)
            {
                child.GetComponent<MeshRenderer>().enabled = false;
            }
            animatorChild.gameObject.SetActive(true);
            animatorChild.SetTrigger("FailTrigger");
            yield return new WaitForSeconds(2f);
            foreach (MeshRenderer child in children)
            {
                child.GetComponent<MeshRenderer>().enabled = true;
                if(child.name.Contains("Cylinder"))
                {
                    child.gameObject.transform.eulerAngles = new Vector3(child.transform.eulerAngles.x, child.transform.eulerAngles.y, -27.68f);
                }
                
            }
            animatorChild.gameObject.SetActive(false);
            _dial1[0] = 0;
            _dial2[0] = 0;
            _dial3[0] = 0;
            _dial4[0] = 0;
        }

        else
        {
            _puzzleAudio.PlayOneShot(winSound);
            MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer child in children)
            {
                child.GetComponent<MeshRenderer>().enabled = false;
            }
            animatorChild.gameObject.SetActive(true);
            animatorChild.SetTrigger("WinTrigger");
            GetComponent<BoxCollider>().enabled = false;
            yield return new WaitForSeconds(4.30f);
            GivePlayerCrank();
            Destroy(GetComponent<PuzzleEscape>());
        }
        _canAnimate = true;
    }
    #endregion

    private void GivePlayerCrank()
    {
        Transform crank = transform.GetChild(6).transform.GetChild(6);
        crank.GetComponent<InteractableObject>().onInteraction.Invoke();
        Destroy(transform.GetChild(6).transform.GetChild(5).gameObject);
    }
}
