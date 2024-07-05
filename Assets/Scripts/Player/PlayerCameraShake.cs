using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _shakeCam;
    private float _shakeIntensity = 1f;
    private float _shakeTime = .2f;

    private float _timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    public bool _shouldShake;
    private void Awake()
    {
        _shakeCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (_shouldShake)
        {
            Shake();
            StartCoroutine(ShakeTime());
        }

        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                StopShake();
            }
        }
    }

    public void Shake()
    {
        _cbmcp = _shakeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = _shakeIntensity;

        _timer = _shakeTime;
    }

    private void StopShake()
    {
        _cbmcp = _shakeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;

        _timer = 0;
    }

    public IEnumerator ShakeTime()
    {
        _shouldShake = true;
        yield return new WaitForSeconds(2.0f);
        _shouldShake = false;
    }
}
