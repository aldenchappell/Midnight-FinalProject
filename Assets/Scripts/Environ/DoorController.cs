using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorController : MonoBehaviour
{
    public bool canBeOpened = true;

    [SerializeField] private float doorInteractionCooldown = 1.5f;

    private Animator _animator;

    private AudioSource _audio;
    [SerializeField] private AudioClip openDoorSound, closeDoorSound;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    public void HandleDoor()
    {
        print("Handling door");
        if (canBeOpened)
        {
            if (!_animator.GetBool("Open") == true)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }
    
    private void OpenDoor()
    {
        _animator.SetBool("Open", true);
        _audio.PlayOneShot(openDoorSound);
        
        StartDoorInteractionCooldown();
    }

    private void CloseDoor()
    {
        _animator.SetBool("Open", false);
        _audio.PlayOneShot(closeDoorSound);
        
        StartDoorInteractionCooldown();
    }

    private void StartDoorInteractionCooldown()
    {
        StartCoroutine(InitiateDoorInteractionCooldown());
    }

    private IEnumerator InitiateDoorInteractionCooldown()
    {
        canBeOpened = false;
        yield return new WaitForSeconds(doorInteractionCooldown);
        canBeOpened = true;
    }
}
