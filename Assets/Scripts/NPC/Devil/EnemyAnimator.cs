using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    void Awake()
    {
        //_animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void SetAnimationTrigger(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    public void ResetAnimationTrigger(string animationName)
    {
        animator.ResetTrigger(animationName);
    }

    public void ResetAllTriggers()
    {
        foreach(var param in animator.parameters)
        {
            if(param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    public void SetAnimationBoolean(string animationName, bool value)
    {
        animator.SetBool(animationName, value);
    }
}