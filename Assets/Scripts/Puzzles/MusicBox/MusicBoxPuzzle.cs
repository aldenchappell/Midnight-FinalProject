using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBoxPuzzle : MonoBehaviour
{
    private Animator _animator;
    private bool _readyToAnim;
    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
    }

    public void ReadyToAnim()
    {
        _readyToAnim = true;
        print("Setting to true");
    }

    public void AnimateMusicBox()
    {
        if(_readyToAnim)
        {
            _animator.SetTrigger("PlayMusicBox");
            Invoke("AnimateMusicBox", 4.04f);
        }
    }
}
