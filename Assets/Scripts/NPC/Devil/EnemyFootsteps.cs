using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFootsteps : MonoBehaviour
{
    private EnemyStateController _enemyStateController;

    private AudioSource _source;
    [SerializeField] private AudioClip[] walkingFootstepClips;
    [SerializeField] private AudioClip[] runningFootstepClips;
    private float _footstepDelay;

    [SerializeField] private float chaseDelay = .7f;
    [SerializeField] private float walkDelay = 2.0f;

    private Coroutine _footstepRoutine;

    private AudioClip[] currentClips;

    [SerializeField] private AudioClip demonLaughSound;

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

    private void OnEnable()
    {
        _footstepRoutine = StartCoroutine(Footsteps());
    }

    private void Update()
    {
        //if the enemy is currently chasing, use the running footsteps, else use the walking footsteps
        currentClips = _enemyStateController.currentState == EnemyStateController.AIState.Chase
            ? runningFootstepClips
            : walkingFootstepClips;
        
        _footstepDelay = _enemyStateController.currentState == EnemyStateController.AIState.Chase
            ? chaseDelay
            : walkDelay;
    }

    private IEnumerator Footsteps()
    {
        while (true)
        {
            yield return new WaitForSeconds(_footstepDelay);
            if (currentClips.Length > 0 && !FindObjectOfType<PlayerDeathController>().isDead)
            {
                AudioClip clip = GetRandomClip(currentClips);
                _source.PlayOneShot(clip, 4.0f);
            }
            else
            {
                _source.PlayOneShot(demonLaughSound);
                break;
            }
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
