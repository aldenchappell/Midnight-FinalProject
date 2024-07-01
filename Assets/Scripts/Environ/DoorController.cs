using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool canBeOpened = true;
    private EnvironmentalSoundController _soundController;
    [SerializeField] private float doorInteractionCooldown = 1.5f;

    private Animator _animator;

    //private AudioSource _audio;
    [SerializeField] private AudioClip openDoorSound, closeDoorSound;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _soundController = FindObjectOfType<EnvironmentalSoundController>();

        if (GetComponent<AudioSource>())
        {
            AudioSource source = GetComponent<AudioSource>();
            Destroy(source);
        }
    }

    public void HandleDoor()
    {
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
        _soundController.transform.position = transform.position;
        _soundController.audioSource.PlayOneShot(openDoorSound);
        
        StartDoorInteractionCooldown();
    }

    private void CloseDoor()
    {
        _animator.SetBool("Open", false);
        _soundController.transform.position = transform.position;
        _soundController.audioSource.PlayOneShot(closeDoorSound);
        
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
