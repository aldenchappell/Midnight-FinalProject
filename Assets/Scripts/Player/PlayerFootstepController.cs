using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootstepController : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip defaultFootstepAudioClip;
    
    
    [SerializeField] private float baseFootstepSpeed = 0.5f;
    [SerializeField] private float crouchFootstepMultiplier = 1.5f;
    [SerializeField] private float sprintFootstepMultiplier = 0.6f;

    private FirstPersonController _firstPersonController;

    private float _footstepTimer = 0.0f;

    private float GetCurrentOffset => _firstPersonController.isCrouching
        ?
        baseFootstepSpeed * crouchFootstepMultiplier
        : _firstPersonController.isSprinting
            ? baseFootstepSpeed * sprintFootstepMultiplier
            : baseFootstepSpeed;

    private GroundType _currentGroundType = null;

    private void Update()
    {
        if (InGameSettingsManager.Instance.enableFootsteps)
        {
            HandleFootsteps();
        }
    }

    private void HandleFootsteps()
    {
        if (!_firstPersonController.Grounded) return;
        if (_firstPersonController.input.move == Vector2.zero) return;

        _footstepTimer -= Time.deltaTime;

        if (_footstepTimer <= 0)
        {
            if (Physics.Raycast(
                    _firstPersonController.cameraRoot.transform.position,
                    Vector3.down,
                    out RaycastHit hitInfo,
                    3))
            {
                if (hitInfo.collider.GetComponent<GroundType>())
                {
                    _currentGroundType = hitInfo.collider.GetComponent<GroundType>();
                    string currentGroundTypeName = _currentGroundType.SO_GroundType.groundTypeName;
                    
                    PlayRandomAudioClip(_currentGroundType.SO_GroundType.groundTypeAudioClips);
                    
                    switch (currentGroundTypeName)
                    {
                        case "Metal":
                            
                            break;
                        case "Carpet":
                            
                            break;
                        case "Concrete":
                            
                            break;
                        case "Hardwood":
                            
                            break;
                        default:
                            
                            Debug.LogError("Error getting the correct ground type name and playing sound" +
                                           "PlayerFootstepController/HandleFootsteps, playing default sound");
                            break;
                    }
                }
                else
                {
                    _currentGroundType = null;
                }
            }
        }
    }

    private void PlayRandomAudioClip(AudioClip[] clips)
    {
        footstepAudioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        _currentGroundType = null;
    }
}
