using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartbeatController : MonoBehaviour
{
    [SerializeField] private AudioSource heartbeatAudioSource;

    [SerializeField] private float minHeartBeatVolume = 0.1f;
    [SerializeField] private float maxHeartBeatVolume = 0.75f;
    [SerializeField] private float volumeChangeSpeed = 1.5f;
    [SerializeField] private float maxDistance = 15.0f;
    [SerializeField] private Image enemyCloseImage;

    private EnemyStateController _enemyStateController;
    private Coroutine _enemyCloseCoroutine;
    
    private FadeUI _fadeUI;
    
    private void Awake()
    {
        heartbeatAudioSource.volume = 0.0f;
        enemyCloseImage.enabled = false;
        _fadeUI = FindObjectOfType<FadeUI>();
        
        Color color = enemyCloseImage.color;
        color.a = Mathf.Clamp(color.a, 0, 43f / 255f);
        enemyCloseImage.color = color;
    }

    private void Update()
    {
        if (_enemyStateController == null || !_enemyStateController.gameObject.activeInHierarchy)
        {
            FindEnemyReference();
            if (_enemyStateController == null)
            {
                return;
            }
        }

        if (GetComponentInParent<PlayerDeathController>().isDead)
        {
            heartbeatAudioSource.enabled = false;
            
            StopAllCoroutines();
            enemyCloseImage.enabled = false;
            
            return;
        }

        if (InGameSettingsManager.Instance.enableHeartbeatSounds)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, _enemyStateController.gameObject.transform.position);
            bool enemyClose = distanceToEnemy <= maxDistance;
            bool isDevilChasing = IsDevilChasing();

            HandleHeartBeat(isDevilChasing, enemyClose, distanceToEnemy);

            if (enemyClose)
            {
                if (_enemyCloseCoroutine == null)
                {
                    //EnemyCloseRoutine();
                }
            }
            else
            {
                if (_enemyCloseCoroutine != null)
                {
                    StopCoroutine(_enemyCloseCoroutine);
                    _enemyCloseCoroutine = null;
                    enemyCloseImage.enabled = false;
                }
            }
        }
    }

    private void FindEnemyReference()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Devil");
        if (enemy != null)
        {
            _enemyStateController = enemy.GetComponent<EnemyStateController>();
        }
    }

    private bool IsDevilChasing() => _enemyStateController.currentState == EnemyStateController.AIState.Chase;

    private void HandleHeartBeat(bool isEnemyChasing, bool isEnemyClose, float distanceToEnemy)
    {
        float enemyRealizationValue = _enemyStateController.GetComponent<EnemyVision>().GetRealizationValue;

        //calc the target heartbeat volume based on both realization value and enemy proximity
        float targetHeartbeatVolume = minHeartBeatVolume;

        if (isEnemyClose)
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

    private void EnemyCloseRoutine()
    {
      
            if (_enemyStateController == null || !_enemyStateController.gameObject.activeInHierarchy)
            {
                enemyCloseImage.enabled = false;
                return;
                //yield break;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, _enemyStateController.gameObject.transform.position);
            float proximityFactor = Mathf.Clamp01(1 - (distanceToEnemy / maxDistance + -5));
            //float flashInterval = Mathf.Lerp(1.0f, 0.1f, proximityFactor);

            // Toggle the image enabled state
            enemyCloseImage.enabled = !enemyCloseImage.enabled;

            // Fade in and out the image very fast
            if (enemyCloseImage.enabled)
            {
                StartCoroutine(_fadeUI.FadeEnemyCloseImage(enemyCloseImage));
            }

            //yield return new WaitForSeconds(flashInterval);
        }
    
}