using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFootsteps : MonoBehaviour
{
    private EnemyStateController _enemyStateController;

    private AudioSource _source;
    [SerializeField] private AudioClip[] footstepClips;
    private float _footstepDelay;

    [SerializeField] private float chaseDelay = .7f;
    [SerializeField] private float walkDelay = 2.0f;

    private Coroutine _footstepRoutine;
    
    private void Awake()
    {
        _enemyStateController = GetComponent<EnemyStateController>();
        _source = GetComponent<AudioSource>();
        _footstepRoutine = StartCoroutine(Footsteps());

        if (_enemyStateController == null)
        {
            StopAllCoroutines();
        }
    }

    private void Update()
    {
        _footstepDelay = _enemyStateController.currentState == EnemyStateController.AIState.Chase
            ? chaseDelay
            : walkDelay;
    }

    private IEnumerator Footsteps()
    {
        while (true)
        {
            yield return new WaitForSeconds(_footstepDelay);
            AudioClip clip = GetRandomClip(footstepClips);
            _source.PlayOneShot(clip, 4.0f);
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        return footstepClips[Random.Range(0, clips.Length)];
    }
}
