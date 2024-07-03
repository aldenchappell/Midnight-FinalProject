using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.SceneManagement;

public class RandomSuspicionEvents : MonoBehaviour
{
    [Header("Audio Related")]
    [SerializeField] AudioClip[] _floorBoardSounds;
    [SerializeField] AudioSource _audioSource;

    [Header("Suspicion Per Event")]
    [SerializeField] int suspicionAmount;

    private PatrolSystemManager _patrolManager;
    private FirstPersonController _FPC;
    private bool _isRunning;

    private void Awake()
    {
        _patrolManager = GameObject.FindAnyObjectByType<PatrolSystemManager>();
        _FPC = GameObject.FindAnyObjectByType<FirstPersonController>();
        _isRunning = false;
    }
    
    private void Update()
    {
        if(!_isRunning && SceneManager.GetActiveScene().name != "LOBBY")
        {
            StartCoroutine("ChanceForPlayerRelatedSuspicionEvent");
        }
    }

    // Chance for suspicion event that uses variables from other player scripts
    private IEnumerator ChanceForPlayerRelatedSuspicionEvent()
    {
        //print("Attempting");
        _isRunning = true;
        float delay;
        float chance = Random.Range(0, 101);
        float speedModifier = _FPC.GetCurrentSpeed * .1f;

        if(speedModifier == 0)
        {
            chance = 0;
        }
        else
        {
            chance += 50 * speedModifier;
        }

        if(chance >= 75)
        {
            //print("Sussing");
            delay = 60f;
            _patrolManager.RaiseSuspicion = suspicionAmount;
            _audioSource.gameObject.transform.position = transform.position;
            int randomIndex = Random.Range(0, _floorBoardSounds.Length);
            _audioSource.PlayOneShot(_floorBoardSounds[randomIndex]);
        }
        else
        {
            delay = 10f;
        }


        yield return new WaitForSeconds(delay);
        _isRunning = false;
    }

    private void StartAttemptingEvents()
    {
        _isRunning = false;
    }

    // Chance for suspicion event that do not use variables from other player scripts
}
