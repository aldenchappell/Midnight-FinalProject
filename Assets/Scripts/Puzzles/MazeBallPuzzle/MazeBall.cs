using System;
using UnityEngine;

public class MazeBall : MonoBehaviour
{
    private MazeBallPuzzle _mazePuzzle;

    private AudioSource _audio;
    [SerializeField] private AudioClip dropSound;
    private void Awake()
    {
        _mazePuzzle = FindObjectOfType<MazeBallPuzzle>();
        _audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        _audio.PlayOneShot(dropSound);
        Debug.Log("hit ground");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MazePuzzleCompletion")) return;

        _mazePuzzle.solved = true;
        _mazePuzzle.TogglePuzzleUI();
        _mazePuzzle.puzzle.CompletePuzzle();
        _mazePuzzle.Complete();
        _mazePuzzle.ResetRotation();
        
        Destroy(gameObject);
    }
}
