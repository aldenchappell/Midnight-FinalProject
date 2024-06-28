using UnityEngine;

public class PlayerArmsAnimationController : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetRunning(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning);
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void SetIdle(bool isIdle)
    {
        animator.SetBool("isIdle", isIdle);
    }

    public void SetCrouching(bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);
    }
}

