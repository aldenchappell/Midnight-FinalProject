using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Puzzles/Puzzle", order = 2)]
public class SO_Puzzle : ScriptableObject
{
    public int stepsInvolved;
    public bool solved; //Used to keep track of whether or not the puzzle is solved - for saving purposes.
    public string puzzleName;
}
