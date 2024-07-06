using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class SoundDetectionRadius : MonoBehaviour
{
    [SerializeField] bool debugEnabled;
    [SerializeField] LayerMask targetLayer;

    private FirstPersonController _FPC;
    private bool _onDelay;
    private float currentSoundRadius;

    private void Awake()
    {
        _FPC = GetComponent<FirstPersonController>();
        _onDelay = false;
    }

    private void Update()
    {
        GenerateSoundRadius(_FPC.GetCurrentSpeed);
    }

    private void GenerateSoundRadius(float currentMoveSpeed)
    {
        float soundRadius;

        if(currentMoveSpeed >= 6)
        {
            currentSoundRadius = 16;
            soundRadius = 16;
        }
        else if(currentMoveSpeed >= 4)
        {
            currentSoundRadius = 8;
            soundRadius = 8;
        }
        else if(currentMoveSpeed >= 2)
        {
            currentSoundRadius = 3.25f;
            soundRadius = 3.25f;
        }
        else
        {
            currentSoundRadius = 0;
            soundRadius = 0;
        }

        if(_FPC.canMove == false)
        {
            currentSoundRadius = 0;
            soundRadius = 0;
        }

        if(soundRadius > 0)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, soundRadius, targetLayer);
            if(targets.Length > 0 && !_onDelay)
            {
                targets[0].GetComponent<EnemySuspicionSystem>().SuspicionTriggered(transform.position, 20);
                _onDelay = true;
                Invoke("ResetDelay", 1f);
            }
        }
        
    }

    private void ResetDelay()
    {
        _onDelay = false;
    }

    private void OnDrawGizmos()
    {
        if(debugEnabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, currentSoundRadius);
        }
    }
}
