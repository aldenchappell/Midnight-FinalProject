using UnityEngine;

public class PlayerHeartbeatController : MonoBehaviour
{
    [SerializeField] private AudioSource heartbeatAudioSource;

    [SerializeField] private float minHeartBeatVolume = 0.1f;
    [SerializeField] private float maxHeartBeatVolume = 0.75f;
    [SerializeField] private float volumeChangeSpeed = 1.5f;
    [SerializeField] private float maxDistance = 15.0f;

    private EnemyStateController _enemyStateController;

    private void Awake()
    {
        heartbeatAudioSource.volume = 0.0f;
        //heartbeatAudioSource.enabled = false;

        _enemyStateController = GameObject.FindGameObjectWithTag("Devil")?.GetComponent<EnemyStateController>();
    }

    private void Update()
    {
        if (InGameSettingsManager.Instance.enableHeartbeatSounds)
        {
            bool isDevilChasing = IsDevilChasing();

            float distanceToEnemy = Vector3.Distance(transform.position, _enemyStateController.gameObject.transform.position);
            bool enemyClose = distanceToEnemy <= maxDistance;

            HandleHeartBeat(isDevilChasing, enemyClose, distanceToEnemy);
        }
    }

    private bool IsDevilChasing() => _enemyStateController.currentState == EnemyStateController.AIState.Chase;

    private void HandleHeartBeat(bool isEnemyChasing, bool isEnemyClose, float distanceToEnemy)
    {
        float enemyRealizationValue = _enemyStateController.GetComponent<EnemyVision>().GetRealizationValue;

        //calc the target heartbeat volume based on both realization value and enemy proximity
        float targetHeartbeatVolume = minHeartBeatVolume;

        if (isEnemyChasing && isEnemyClose)
        {
            //calc the proximity factor based on distance
            float enemyProximityFactor = Mathf.Clamp01(1 - (distanceToEnemy / maxDistance));

            //l between min and max heartbeat volume based on proximity factor
            targetHeartbeatVolume = Mathf.Lerp(minHeartBeatVolume, maxHeartBeatVolume, enemyProximityFactor);
            
        }

        //heartbeat volume is based on realization value only when the enemy is not chasing
        if (!isEnemyChasing)
        {
            // Smoothly interpolate the current volume to the minimum volume
            heartbeatAudioSource.volume = Mathf.Lerp(
                heartbeatAudioSource.volume,
                minHeartBeatVolume,
                Time.deltaTime * volumeChangeSpeed);
            
            //set volume to 0 after it has reached the minimum volume and the realization value is decreasing
            if (heartbeatAudioSource.volume <= minHeartBeatVolume + 0.025f && enemyRealizationValue < 20)
            {
                heartbeatAudioSource.volume = 0.0f;
            }
        }
        else
        {
            //based on realization value when the enemy is chasing
            targetHeartbeatVolume *= (enemyRealizationValue / 20f); //should calculate between 0 and 1
            
            heartbeatAudioSource.volume = Mathf.Lerp(
                heartbeatAudioSource.volume,
                targetHeartbeatVolume,
                Time.deltaTime * volumeChangeSpeed);
        }
    }
}