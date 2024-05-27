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
    }


    public void AdjustTargetDialLetter(int _dialNumber)
    {
        switch (_dialNumber)
        {
            case (10):
                _dial1[0] += 1;
                break;
            case (11):
                _dial1[0] += -1;
                break;
            case (20):
                _dial2[0] += 1;
                break;
            case (21):
                _dial2[0] += -1;
                break;
            case (30):
                _dial3[0] += 1;
                break;
            case (31):
                _dial3[0] += -1;
                break;
            case (40):
                _dial4[0] += 1;
                break;
            case (41):
                _dial4[0] += -1;
                break;
        }
        print(_dial1[0] + " " + _dial2[0] + " " + _dial3[0] + " " + _dial1[4]);
        CheckForCorrectCombination();
    }

    private void CheckForCorrectCombination()
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
        }
    }

    public void ActivatePuzzleUI()
    {
        puzzleUI.SetActive(true);
    }
}
