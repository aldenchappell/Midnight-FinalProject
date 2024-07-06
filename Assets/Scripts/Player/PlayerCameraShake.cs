using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerCameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _shakeCam; 
    [SerializeField] private NoiseSettings handheldNormalMild;
    [SerializeField] private NoiseSettings shakeProfile;
    private CinemachineBasicMultiChannelPerlin _perlinChannel;
    
    private const float ShakeIntensity = 3f;
    private const float ShakeTime = 3.1f;
    private float _timer;

    [SerializeField] private Transform debrisPos;
    [SerializeField] private GameObject fallingDebris;
    [SerializeField] private AudioClip fallingDebrisClip;
    private AudioSource _audio;
    private bool _shaking;
    private GameObject _debris;
    private void Awake()
    {
        _shakeCam = GetComponent<CinemachineVirtualCamera>();
        _audio = GetComponentInChildren<AudioSource>();
        
        if (_shakeCam != null)
        {
            _perlinChannel = _shakeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    private void Update()
    {
        if (InGameSettingsManager.Instance.enableShaking)
        {
            #region Cheats
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                TriggerShake();
            }

            if (_shaking && Input.GetKeyDown(KeyCode.L))
            {
                StopShake();
            }
#endif
            #endregion
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    StopShake();
                }
            }
        }
        
        else
        {
            StopShake();
        }
    }

    public void TriggerShake()
    {
        if (_perlinChannel != null && !_shaking)
        {
            _perlinChannel.m_NoiseProfile = shakeProfile;
            _perlinChannel.m_AmplitudeGain = ShakeIntensity;
            _timer = ShakeTime;
            _shaking = true;
            StartCoroutine(FallingDebris());

            Debug.Log("starting shake");
            
            if (_audio != null)
                _audio.PlayOneShot(fallingDebrisClip);
        }
    }

    private void StopShake()
    {
        if (_perlinChannel != null)
        {
            _perlinChannel.m_NoiseProfile = handheldNormalMild;
            _perlinChannel.m_AmplitudeGain = 0f;

            _shaking = false;
            
            Debug.Log("stopping shake");
        }
    }

    private IEnumerator FallingDebris()
    {
        _debris = Instantiate(fallingDebris, debrisPos.position, debrisPos.localRotation);
        var debrisParticleSystem = _debris.GetComponent<ParticleSystem>();
        _debris.transform.SetParent(debrisPos);
        debrisParticleSystem.Play();
        
        yield return new WaitForSeconds(ShakeTime);
        
        var emissionModule = debrisParticleSystem.emission;
        float extendedDebrisLifetime = 1.0f; 
        float emissionRate = emissionModule.rateOverTime.constant;
        float emissionRateReduction = emissionRate / extendedDebrisLifetime;

        for (float i = 0; i < extendedDebrisLifetime; i += Time.deltaTime)
        {
            emissionModule.rateOverTime = Mathf.Max(0.0f, emissionRate - (emissionRateReduction * i));
            yield return null;
        }
        
        debrisParticleSystem.Stop();
        Destroy(_debris, debrisParticleSystem.main.startLifetime.constant);
    }
}