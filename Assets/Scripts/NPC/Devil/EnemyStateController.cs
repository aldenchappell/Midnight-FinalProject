using UnityEngine;
using UnityEngine.AI;

public class EnemyStateController : MonoBehaviour
{
    // Enemy State Parameters
    public enum AIState { Roam, Chase, Patrol };
    public AIState currentState;
    private AIState _previousState;

    [Header("Movement Speed Values")]
    [SerializeField] private float roamSpeed = 1.85f;
    [SerializeField] private float chaseSpeed = 2.25f;
    [SerializeField] private float patrollingSpeed = 2.0f;
    
    [Space(10)]
    
    
    // Script References
    private EnemyVision _enemyVision;
    private SetMovement _setMovement;
    private NavMeshAgent _agent;
    private EnemyAnimator _animator;
    private EnemySuspicionSystem _suspicion;


    private PlayerDeathController _playerDeathController;
    private void Awake()
    {
        _enemyVision = GetComponent<EnemyVision>();
        _setMovement = GetComponent<SetMovement>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<EnemyAnimator>();
        _suspicion = GetComponent<EnemySuspicionSystem>();
        _playerDeathController = FindObjectOfType<PlayerDeathController>();
        
        //ApplyCurrentStateBehaviour();
    }

    private void Start()
    {
        currentState = AIState.Roam; 
        _animator.SetAnimationTrigger(currentState.ToString());

        FindObjectOfType<EnemyCloseUI>().shouldHandleImage = true;
    }

    private void Update()
    {
        DetermineCurrentState();
        ApplyCurrentStateBehaviour();
        //print(currentState);
    }

    private void DetermineCurrentState()
    {
        _previousState = currentState;
        if (_enemyVision.targetsLockedIn.Count > 0)
        {
            currentState = AIState.Chase;
        }
        else if (_suspicion.GetSuspicionValue > 20)
        {
            currentState = AIState.Patrol;
        }
        else if(currentState == AIState.Patrol && _suspicion.GetSuspicionValue > 0)
        {
            currentState = AIState.Patrol;
        }
        else
        {
            currentState = AIState.Roam;
        }
        _animator.ResetAllTriggers();
        _animator.SetAnimationTrigger(currentState.ToString());
    }

    private void ApplyCurrentStateBehaviour()
    {
       
        if(_playerDeathController.isDead)
        {
            _animator.SetAnimationTrigger("Idle");
            _agent.speed = 0;
            return;
        }
        
        if(_previousState != currentState)
        {
            _agent.ResetPath();
        }
        if (currentState == AIState.Chase)
        {
            Transform target = _enemyVision.targetsLockedIn[0].transform;
            _agent.speed = chaseSpeed;
            _setMovement.SetCurrentMovementState("Chasing", target.position);
        }
        else if (currentState == AIState.Patrol)
        {
            Vector3 lastKnownPosition = _suspicion.lastSusPosition;
            _agent.speed = patrollingSpeed;
            _setMovement.SetCurrentMovementState("Patrolling", lastKnownPosition);
        }
        else
        {
            _agent.speed = roamSpeed;
            _setMovement.SetCurrentMovementState("Roaming", Vector3.zero);
        }
        
        //Set animation state based on current state (Animation triggers should be named exactly the same
        //as the current state's name for this to properly work. - Alden)
        
    }

    public bool CheckForChaseState()
    {
        return currentState == AIState.Chase;
    }

    public void ResetSuspicionValue()
    {
        _suspicion.suspicionValue = 0;
    }
}