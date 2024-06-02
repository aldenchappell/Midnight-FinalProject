using UnityEngine;

public class PlayerHeartbeatController : MonoBehaviour
{
    [SerializeField] private AudioSource heartbeatAudioSource;

    [SerializeField] private float minHeartBeatVolume = 0.1f;
    [SerializeField] private float maxHeartBeatVolume = 0.75f;
    [SerializeField] private float volumeChangeSpeed = 1.5f;
    [SerializeField] private float maxDistance = 15.0f;

    private EnemyStateController enemyStateController;

    private void Awake()
    {
        heartbeatAudioSource.volume = 0.0f;
        heartbeatAudioSource.enabled = false;

        enemyStateController = GameObject.FindGameObjectWithTag("Devil").GetComponent<EnemyStateController>();
    }

    private void Update()
    {
        bool isDevilChasing = IsDevilChasing();

        float distanceToEnemy = Vector3.Distance(transform.position, enemyStateController.gameObject.transform.position);
        bool enemyClose = distanceToEnemy <= maxDistance;

        HandleHeartBeat(isDevilChasing, enemyClose, distanceToEnemy);
    }

    private bool IsDevilChasing() => enemyStateController.currentState == EnemyStateController.AIState.Chase;

    private void HandleHeartBeat(bool isEnemyChasing, bool enemyClose, float distanceToEnemy)
    {
        float targetHeartbeatVolume = minHeartBeatVolume;

        if (isEnemyChasing && enemyClose)
        {
            float enemyProximityFactor = Mathf.Clamp01(1 - (distanceToEnemy / maxDistance));
            targetHeartbeatVolume = Mathf.Lerp(minHeartBeatVolume, maxHeartBeatVolume, enemyProximityFactor);
        }
        
        //set the volume based on the distance of the player to the enemy 
        heartbeatAudioSource.volume = Mathf.Lerp(
            heartbeatAudioSource.volume,
            targetHeartbeatVolume,
            Time.deltaTime * volumeChangeSpeed);

        // Enable the audio source if volume is above the minimum volume
        if (heartbeatAudioSource.volume > minHeartBeatVolume + .01f)
        {
            heartbeatAudioSource.enabled = true;
        }
        else if (!isEnemyChasing && heartbeatAudioSource.volume <= minHeartBeatVolume + 0.01f)
        {
            //disable the audio source after it has reached the minimum volume and the enemy is not chasing
            heartbeatAudioSource.enabled = false;
            heartbeatAudioSource.volume = 0.0f;
        }

        Debug.Log(heartbeatAudioSource.volume);
    }
}