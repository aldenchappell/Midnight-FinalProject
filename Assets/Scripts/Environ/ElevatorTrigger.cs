using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ElevatorTrigger : MonoBehaviour
{
    public UnityEvent onElevatorTrigger;

    private bool _isOnCooldown;
    private bool _triggered;
    
    private AudioSource _audio;
    [SerializeField] private AudioClip onCooldownAlertSound;

    private ElevatorController _elevatorController;
    
    private void Awake()
    {
        _audio = GetComponentInParent<AudioSource>();
        _elevatorController = GetComponentInParent<ElevatorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        
        if (_isOnCooldown)
        {
            PlayCooldownAlert();
            //Debug.Log("On cooldown or level isnt completed");
        }
        else if(!_triggered)
        {
            var opened = _elevatorController.GetComponent<Animator>().GetBool("Open");
            if (opened)
            {
                onElevatorTrigger.Invoke();
                StartCoroutine(DestroyAfterDelay());
            }
        }
    }
    
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }

    private void PlayCooldownAlert()
    {
        if (onCooldownAlertSound != null && _audio != null)
        {
            _audio.PlayOneShot(onCooldownAlertSound);
        }
    }
}