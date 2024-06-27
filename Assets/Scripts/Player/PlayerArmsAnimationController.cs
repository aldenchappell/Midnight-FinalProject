using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerArmsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    private FirstPersonController _firstPersonController;
    private PlayerDualHandInventory _playerInventory;
    private PlayerInteractableController _playerInteractableController;
    
    [HideInInspector] public int idle;
    [HideInInspector] public int walking;
    [HideInInspector] public int walkingWithItem;
    [HideInInspector] public int running;
    [HideInInspector] public int runningWithItem;
    [HideInInspector] public int pickupItem;
    [HideInInspector] public int openDoor;

    public List<int> animationIDs = new List<int>();
    private void Awake()
    {
        _firstPersonController = FindObjectOfType<FirstPersonController>();
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>();
        _playerInteractableController = FindObjectOfType<PlayerInteractableController>();
        // _playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void Update()
    {
        if (_firstPersonController.input.move == Vector2.zero)
        {
            HandleArmAnimation(idle);
            return;
        }

        if (_firstPersonController.isSprinting)
        {
            HandleArmAnimation(running);
            return;
        }
        
        if (_firstPersonController.isSprinting
            && _playerInventory.GetInventory.Length > 0)
        {
            HandleArmAnimation(runningWithItem);
            return;
        }

        if (_firstPersonController.input.move != Vector2.zero && !_firstPersonController.isSprinting)
        {
            HandleArmAnimation(walking);
            return;
        }
        
        if (_firstPersonController.input.move != Vector2.zero 
            && !_firstPersonController.isSprinting
            && _playerInventory.GetInventory.Length > 0)
        {
            HandleArmAnimation(walkingWithItem);
            return;
        }

        if (_playerInteractableController.IsLookingAtInteractableObject(_playerInteractableController.interactableObject.gameObject))
        {
            HandleArmAnimation(pickupItem);
            return;
        }
    }

    public void HandleArmAnimation(int currentAnimID)
    {
        _playerAnimator.SetBool(idle, false);
        _playerAnimator.SetBool(walking, false);
        _playerAnimator.SetBool(walkingWithItem, false);
        _playerAnimator.SetBool(running, false);
        _playerAnimator.SetBool(runningWithItem, false);
        _playerAnimator.SetBool(pickupItem, false);

        _playerAnimator.SetBool(currentAnimID, true);

        foreach (var anim in animationIDs)
        {
            if (anim != currentAnimID)
            {
                _playerAnimator.SetBool(currentAnimID, false);
            }
            else
            {
                _playerAnimator.SetBool(currentAnimID, true);
            }
        }
    }
    
    /// <summary>
    /// Call this method any time you need to play or set the player's animation state.
    /// Example use case: playerArms.SetArmAnimationState(playerArms.walking);
    /// This use case will set the players animation to the walking state.
    /// </summary>
    /// <param name="animID"></param>
    // public void SetArmAnimationState(int animID)
    // {
    //     _playerAnimator.SetBool(idle, false);
    //     _playerAnimator.SetBool(walking, false);
    //     _playerAnimator.SetBool(walkingWithItem, false);
    //     _playerAnimator.SetBool(running, false);
    //     _playerAnimator.SetBool(runningWithItem, false);
    //     _playerAnimator.SetBool(pickupItem, false);
    //     
    //     switch (animID)
    //     {
    //         case var id when id == idle:
    //             _playerAnimator.SetBool(idle, true);
    //            DetermineFalseAnimations(idle);
    //             break;
    //         case var id when id == walking:
    //             _playerAnimator.SetBool(walking, true);
    //             DetermineFalseAnimations(walking);
    //             break;
    //         case var id when id == walkingWithItem:
    //             _playerAnimator.SetBool(walkingWithItem, true);
    //             DetermineFalseAnimations(walkingWithItem);
    //             break;
    //         case var id when id == running:
    //             _playerAnimator.SetBool(running, true);
    //             DetermineFalseAnimations(running);
    //             break;
    //         case var id when id == runningWithItem:
    //             _playerAnimator.SetBool(runningWithItem, true);
    //             DetermineFalseAnimations(runningWithItem);
    //             break;
    //         case var id when id == pickupItem:
    //             _playerAnimator.SetBool(pickupItem, true);
    //             DetermineFalseAnimations(pickupItem);
    //             break;
    //         case var id when id == openDoor:
    //             _playerAnimator.SetBool(openDoor, true);
    //             DetermineFalseAnimations(openDoor);
    //             break;
    //         default:
    //             Debug.Log("Error setting player arm animation state. PlayerArmsAnimationController/SetArmAnimationState.");
    //             _playerAnimator.SetBool(walking, true);
    //             break;
    //     }
    // }

    private void DetermineFalseAnimations(int correctID)
    {
        List<int> animationIDs = new List<int>();

        animationIDs.Add(idle);
        animationIDs.Add(walking);
        animationIDs.Add(walkingWithItem);
        animationIDs.Add(running);
        animationIDs.Add(runningWithItem);
        animationIDs.Add(pickupItem);
        animationIDs.Add(openDoor);

        for (int i = 0; i < animationIDs.Count; i++)
        {
            if(!i.ToString().Contains(correctID.ToString()))
            {
                foreach (var id in animationIDs)
                {
                    _playerAnimator.SetBool(id, false);
                }
                Debug.Log("Setting " + i + " to false");
                //return;
            }
            else
            {
                _playerAnimator.SetBool(i, true);
                Debug.Log("Setting " + i + " to true");
            }
        }
    }
    
    private void AssignAnimationIDs()
    {
        idle = Animator.StringToHash("Idle");
        walking = Animator.StringToHash("Walking");
        walkingWithItem = Animator.StringToHash("WalkWithItem");
        running = Animator.StringToHash("Running");
        runningWithItem = Animator.StringToHash("RunWithItem");
        pickupItem = Animator.StringToHash("PickupItem");
        openDoor = Animator.StringToHash("OpenDoor");
        animationIDs.Add(idle);
        animationIDs.Add(walking);
        animationIDs.Add(walkingWithItem);
        animationIDs.Add(running);
        animationIDs.Add(runningWithItem);
        animationIDs.Add(pickupItem);
        animationIDs.Add(openDoor);
    }
}
