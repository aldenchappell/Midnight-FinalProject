using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTableDrawer : MonoBehaviour
{
    private Animator _animator;
    private bool _isOpen;
    private bool _canAnimate;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isOpen = false;
        _canAnimate = true;
    }

    public void SetDrawerAnimTrigger()
    {
        if(_canAnimate)
        {
            _canAnimate = false;
            if (_isOpen)
            {
                print("Interacting Close");
                _animator.SetTrigger("Close");
            }
            else
            {
                print("Interacting Open");
                _animator.SetTrigger("Open");
            }
            _isOpen = !_isOpen;
            Invoke("ChangeAnimateBool", 1f);
        }
    }

    private void ChangeAnimateBool()
    {
        _canAnimate = true;
    }
}
