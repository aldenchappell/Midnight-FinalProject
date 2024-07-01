using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerArmsAnimationController : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    private bool _shouldPickup = true;

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

    public void SetPickingUp(bool isPickingUp)
    {
        if (_shouldPickup)
        {
            animator.SetBool("isPickingUp", isPickingUp);
            StartCoroutine(ShouldSetPickingUpTimer());
        }
        
    }

    public void SetIsRunningWithItem(bool isRunningWithItem)
    {
        animator.SetBool("isRunningWithItem", isRunningWithItem);
    }

    public void SetPickedUp()
    {
        animator.SetTrigger("pickedUp");
    }

    private IEnumerator ShouldSetPickingUpTimer()
    {
        _shouldPickup = false;
        yield return new WaitForSeconds(1.2f);
        _shouldPickup = true;
    }
}