using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorTrigger : MonoBehaviour
{
    public UnityEvent onElevatorTrigger;
    public UnityEvent onElevatorTriggerExit;

    private bool _isOnCooldown;

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
        }
        else
        {
            ActivateElevator();
        }
    }

    private void ActivateElevator()
    {
        if (_elevatorController != null)
        {
            _elevatorController.OpenElevator();
            onElevatorTrigger.Invoke();
            StartCoroutine(ElevatorCooldown());
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