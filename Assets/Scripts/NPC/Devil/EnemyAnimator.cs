using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void SetAnimationTrigger(string animationName)
    {
        _animator.SetTrigger(animationName);
    }

    public void ResetAnimationTrigger(string animationName)
    {
        _animator.ResetTrigger(animationName);
    }

    public void SetAnimationBoolean(string animationName, bool value)
    {
        _animator.SetBool(animationName, value);
    }
}