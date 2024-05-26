using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootstepController : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    
    //May be used just in case the current ground the player is on does not have a ground type.
    [SerializeField] private AudioClip[] defaultFootstepAudioClips;

    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] private float footstepRaycastDistance = 3.0f;
    
    [SerializeField] private float baseFootstepSpeed = 0.5f;
    [SerializeField] private float crouchFootstepMultiplier = 1.5f;
    [SerializeField] private float sprintFootstepMultiplier = 0.6f;

    private FirstPersonController _firstPersonController;

    private float _footstepTimer = 0.0f;

    //This float is used to determine which movement state the player is in, and will determine the rate in which
    //the footstep sounds will play. (Crouching, Walking, Running).
    private float GetCurrentFootstepOffset => _firstPersonController.isCrouching
        ?
        baseFootstepSpeed * crouchFootstepMultiplier
        : _firstPersonController.isSprinting
            ? baseFootstepSpeed * sprintFootstepMultiplier
            : baseFootstepSpeed;

    //Keep track of the current ground type 
    private GroundType _currentGroundType;

    private void Awake()
    {
        _firstPersonController = GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (InGameSettingsManager.Instance.enableFootsteps)
        {
            HandleFootsteps();
        }
    }

    private void HandleFootsteps()
    {
        //Ensure the player is grounded and moving before playing footstep sounds.
        if (!_firstPersonController.Grounded) return;
        if (_firstPersonController.input.move == Vector2.zero) return;

        _footstepTimer -= Time.deltaTime;

        if (_footstepTimer <= 0)
        {   //Send a raycast from the cameraRoot to the ground to get the groundType component attached to the current
            //ground type. Also ensure that the ground is on the ground layer for further bug prevention.
            if (Physics.Raycast(
                    _firstPersonController.cameraRoot.transform.position,
                    Vector3.down,
                    out RaycastHit hitInfo,
                    footstepRaycastDistance,
                    groundLayer))
            {
                GroundType groundType = hitInfo.collider.GetComponent<GroundType>();
                if (groundType != null)
                {
                    _currentGroundType = groundType;
                    PlayRandomAudioClip(_currentGroundType.SO_GroundType.groundTypeAudioClips);
                }
                else
                {
                    _currentGroundType = null;
                }
            }

            //Start the footstep timer based on the current movement state the player is in.
            _footstepTimer = GetCurrentFootstepOffset;
        }
    }

    private void PlayRandomAudioClip(AudioClip[] clips)
    {
        if (footstepAudioSource == null) return;

        if (clips == null || clips.Length == 0)
        {
            //If the current ground type does not have any audio clips, or the ground isn't detected, play a 
            //set of default audio clips.
            clips = defaultFootstepAudioClips;
            
            Debug.Log("No audio clips assigned to the current ground type." +
                      "Playing a default footstep sound.");
        }

        
        //Plays a random sound from the current ground type's audio clip array at the specified volume value.
        footstepAudioSource.PlayOneShot(clips[Random.Range(0, clips.Length)],
            _currentGroundType.SO_GroundType.groundTypeVolumeValue);
        
        //reset the current ground type after the sound clip has played.
        _currentGroundType = null;
    }
}
