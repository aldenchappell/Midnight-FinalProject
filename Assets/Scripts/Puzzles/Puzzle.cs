using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public SO_Puzzle SO_puzzle;

    private int _stepsSolved;
    private void Start()
    {
        // if the puzzle has already been solved, do not spawn it in at runtime.
        gameObject.SetActive(SO_puzzle.solved);
    }

    //Check to see if the amount of steps have been successfully solved for this puzzle
    public bool IsSolved() => _stepsSolved == SO_puzzle.stepsInvolved;
}
