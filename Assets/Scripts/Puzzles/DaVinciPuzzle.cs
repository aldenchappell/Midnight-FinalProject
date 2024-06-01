using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class DaVinciPuzzle : MonoBehaviour
{
    [Header("Letters in Da Vinci Code")]
    [SerializeField] string[] letters;
    [Header("Letters for passcode in order")]
    [SerializeField] int[] passcode;
    [Header("Da Vinci Puzzle UI")]
    [SerializeField] GameObject puzzleUI;
    [Header("Music Crank Piece Prefab")]
    [SerializeField] GameObject crankPref;
    [Header("Animator Child Object")]
    [SerializeField] Animator animatorChild;
    [Header("AudioFiles")]
    [SerializeField] AudioClip failSound;
    [SerializeField] AudioClip winSound;

    private GlobalCursorManager _cursor;
    private FirstPersonController _FPC;
    private AudioSource _puzzleAudio;


    private int[] _dial1;
    private int[] _dial2;
    private int[] _dial3;
    private int[] _dial4;

    private void Awake()
    {
        GlobalCursorManager.Instance = _cursor;
        _FPC = GameObject.FindFirstObjectByType<FirstPersonController>();
        _puzzleAudio = GetComponent<AudioSource>();
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


    #region Adjusting Dials
    public void AdjustTargetDialLetter(int _dialNumber)
    {
        switch (_dialNumber)
        {
            case (10):
                _dial1[0] += 1;

                if (_dial1[0] > 25)
                {
                    _dial1[0] = 0;
                }

                break;
            case (11):
                _dial1[0] += -1;

                if (_dial1[0] < 0)
                {
                    _dial1[0] = 25;
                }
                break;
            case (20):
                _dial2[0] += 1;

                if (_dial2[0] > 25)
                {
                    _dial2[0] = 25;
                }
                break;
            case (21):
                _dial2[0] += -1;

                if (_dial2[0] < 0)
                {
                    _dial2[0] = 25;
                }
                break;
            case (30):
                _dial3[0] += 1;

                if (_dial3[0] > 25)
                {
                    _dial3[0] = 0;
                }
                break;
            case (31):
                _dial3[0] += -1;

                if (_dial3[0] < 0)
                {
                    _dial3[0] = 25;
                }
                break;
            case (40):
                _dial4[0] += 1;

                if (_dial4[0] > 25)
                {
                    _dial4[0] = 0;
                }
                break;
            case (41):
                _dial4[0] += -1;

                if (_dial4[0] < 0)
                {
                    _dial4[0] = 25;
                }
                break;
        }
        print(_dial1[0] + " " + _dial2[0] + " " + _dial3[0] + " " + _dial4[0]);
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
            print("Puzzle Solved");
            ActivatePuzzleUI();
            StartCoroutine(TriggerAnimation(false));
            Instantiate(crankPref, new Vector3(transform.position.x + 2, transform.position.y, transform.position.z), Quaternion.identity);
        }
        else
        {
            print("Puzzle Failed");
            StartCoroutine(TriggerAnimation(true));
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
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _FPC.ToggleCanMove();
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
        }
    }
    #endregion
}
