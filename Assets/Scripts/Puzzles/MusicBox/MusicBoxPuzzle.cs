using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBoxPuzzle : MonoBehaviour
{ 
    private Animator _animator;

    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
    }


    public void AnimateMusicBox()
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        _animator.SetTrigger("PlayMusicBox");
        Invoke("LoopAnimation", 4.04f);
        
    }
    private void LoopAnimation()
    {
        _animator.SetTrigger("PlayMusicBox");
        GetComponent<Puzzle>().CompletePuzzle();
        LevelCompletionManager.Instance.OnKeySpawn();
    }
}
