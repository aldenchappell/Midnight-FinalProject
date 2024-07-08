using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerArmsAnimationController : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    //public Transform rightHand;
    //public Transform[] rightFingerTargets;
    //public MultiAimConstraint[] fingerConstraints;

    //private GameObject _heldObject;
    //private bool _isHoldingObject = false;

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
    
    public void SetWalkingWithItem(bool isWalkingWithItem)
    {
        animator.SetBool("isWalkingWithItem", isWalkingWithItem);
    }

    public void SetIdle(bool isIdle)
    {
        animator.SetBool("isIdle", isIdle);
    }

    public void SetPickingUp(bool isPickingUp)
    {
        animator.SetBool("isPickingUp", isPickingUp);
    }

    public void SetIsRunningWithItem(bool isRunningWithItem)
    {
        animator.SetBool("isRunningWithItem", isRunningWithItem);
    }

    public void SetPickedUp()
    {
        animator.SetTrigger("pickedUp");
    }
}