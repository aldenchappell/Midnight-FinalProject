using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ElevatorTrigger : MonoBehaviour
{
    public UnityEvent onElevatorTrigger;
    public UnityEvent onElevatorTriggerExit;

    private bool _isOnCooldown;
    private bool _triggered;
    
    private AudioSource _audio;
    [SerializeField] private AudioClip onCooldownAlertSound;

    private ElevatorController _elevatorController;

    private KeyController _key;

    private void Awake()
    {
        _audio = GetComponentInParent<AudioSource>();
        _elevatorController = GetComponentInParent<ElevatorController>();
        _key = GameObject.FindWithTag("KeyWTag").GetComponent<KeyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        
        
        if (_isOnCooldown)
        {
            PlayCooldownAlert();
            Debug.Log("On cooldown or level isnt completed");
        }
        else if(!_triggered)
        {
            
            ActivateElevator();
        }
    }

    private void ActivateElevator()
    {
        if (_elevatorController != null 
            && LevelCompletionManager.Instance.IsLevelCompleted(SceneManager.GetActiveScene().name)
            && _key.collected)
        {
            onElevatorTrigger.Invoke();
            _triggered = true;
        }
    }

    private IEnumerator ElevatorCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(3.5f);
        if (_elevatorController != null)
        {
            _elevatorController.CloseElevator();
            onElevatorTriggerExit.Invoke(); // Invoke this if you need to trigger any other events when the door closes
        }
        _isOnCooldown = false;
    }

    private void PlayCooldownAlert()
    {
        if (onCooldownAlertSound != null && _audio != null)
        {
            _audio.PlayOneShot(onCooldownAlertSound);
        }
    }
}