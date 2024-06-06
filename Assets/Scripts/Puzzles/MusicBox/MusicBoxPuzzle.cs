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
        if(transform.GetChild(1).name == "CrankV4")
        {
            _animator.SetTrigger("PlayBox");
        }
    }
}
