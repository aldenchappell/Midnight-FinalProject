using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaVinciPuzzle : MonoBehaviour
{
    [Header("Letters in Da Vinci Code")]
    [SerializeField] string[] letters;
    [Header("Letters for passcode in order")]
    [SerializeField] int[] passcode;
    [Header("Da Vinci Puzzle UI")]
    [SerializeField] GameObject puzzleUI;

    private GlobalCursorManager _cursor;

    private int[] _dial1;
    private int[] _dial2;
    private int[] _dial3;
    private int[] _dial4;

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

        GlobalCursorManager.Instance = _cursor;
    }


    #region Adjusting Dials
    public void AdjustTargetDialLetter(int _dialNumber)
    {
        switch (_dialNumber)
        {
            case (10):
                _dial1[0] += -1;

                if (_dial1[0] < 0)
                {
                    _dial1[0] = 26;
                }

                break;
            case (11):
                _dial1[0] += 1;

                if (_dial1[0] > 26)
                {
                    _dial1[0] = 0;
                }

                break;
            case (20):
                _dial2[0] += -1;

                if (_dial2[0] < 0)
                {
                    _dial2[0] = 26;
                }
                break;
            case (21):
                _dial2[0] += 1;

                if (_dial2[0] > 26)
                {
                    _dial2[0] = 0;
                }

                break;
            case (30):
                _dial3[0] += -1;

                if (_dial3[0] < 0)
                {
                    _dial3[0] = 26;
                }
                break;
            case (31):
                _dial3[0] += 1;

                if (_dial3[0] > 26)
                {
                    _dial3[0] = 0;
                }
                break;
            case (40):
                _dial4[0] += -1;

                if (_dial4[0] < 0)
                {
                    _dial4[0] = 26;
                }
                break;
            case (41):
                _dial4[0] += 1;

                if (_dial4[0] > 26)
                {
                    _dial4[0] = 0;
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
        }
        else
        {
            print("Puzzle Failed");
        }
    }

    public void ActivatePuzzleUI()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        if(puzzleUI.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
    #endregion

    #region Rotate Dials
    public void RotateUp(GameObject dial)
    {
        dial.transform.eulerAngles = new Vector3(dial.transform.eulerAngles.x, dial.transform.eulerAngles.y, dial.transform.eulerAngles.z - 10);
    }

    public void RotateDown(GameObject dial)
    {
        dial.transform.eulerAngles = new Vector3(dial.transform.eulerAngles.x, dial.transform.eulerAngles.y, dial.transform.eulerAngles.z - 10);
    }
    #endregion
}
