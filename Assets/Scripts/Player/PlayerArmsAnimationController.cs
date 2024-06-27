using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmsAnimationController : MonoBehaviour
{
    private Animator _playerAnimator;

    [HideInInspector] public int idle;
    [HideInInspector] public int walking;
    [HideInInspector] public int walkingWithItem;
    [HideInInspector] public int running;
    [HideInInspector] public int runningWithItem;
    [HideInInspector] public int pickupItem;
    [HideInInspector] public int openDoor;
    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }
    
    
    /// <summary>
    /// Call this method any time you need to play or set the player's animation state.
    /// Example use case: playerArms.SetArmAnimationState(playerArms.walking);
    /// This use case will set the players animation to the walking state.
    /// </summary>
    /// <param name="animID"></param>
    public void SetArmAnimationState(int animID)
    {
        _playerAnimator.SetBool(idle, false);
        _playerAnimator.SetBool(walking, false);
        _playerAnimator.SetBool(walkingWithItem, false);
        _playerAnimator.SetBool(running, false);
        _playerAnimator.SetBool(runningWithItem, false);
        _playerAnimator.SetBool(pickupItem, false);
        
        switch (animID)
        {
            case var id when id == idle:
                _playerAnimator.SetBool(idle, true);
               DetermineFalseAnimations(idle);
                break;
            case var id when id == walking:
                _playerAnimator.SetBool(walking, true);
                DetermineFalseAnimations(walking);
                break;
            case var id when id == walkingWithItem:
                _playerAnimator.SetBool(walkingWithItem, true);
                DetermineFalseAnimations(walkingWithItem);
                break;
            case var id when id == running:
                _playerAnimator.SetBool(running, true);
                DetermineFalseAnimations(running);
                break;
            case var id when id == runningWithItem:
                _playerAnimator.SetBool(runningWithItem, true);
                DetermineFalseAnimations(runningWithItem);
                break;
            case var id when id == pickupItem:
                _playerAnimator.SetBool(pickupItem, true);
                DetermineFalseAnimations(pickupItem);
                break;
            case var id when id == openDoor:
                _playerAnimator.SetBool(openDoor, true);
                DetermineFalseAnimations(openDoor);
                break;
            default:
                Debug.Log("Error setting player arm animation state. PlayerArmsAnimationController/SetArmAnimationState.");
                _playerAnimator.SetBool(walking, true);
                break;
        }
    }

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
    }
}
