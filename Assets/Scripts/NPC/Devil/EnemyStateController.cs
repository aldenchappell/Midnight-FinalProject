using UnityEngine;
using UnityEngine.AI;

public class EnemyStateController : MonoBehaviour
{
    // Enemy State Parameters
    enum AIState { Roam, Chase, Patrol };
    private AIState _currentState;
    private AIState _previousState;

    [Header("Movement Speed Values")]
    [SerializeField] private float roamSpeed = 1.85f;
    [SerializeField] private float chaseSpeed = 2.25f;
    [SerializeField] private float patrollingSpeed = 2.0f;

    // Script References
    private EnemyVision _enemyVision;
    private RandomMovementController _randomMovement;
    private NavMeshAgent _agent;
    private EnemyAnimator _animator;
    private EnemySuspicionSystem _suspicion;
    
    private void Awake()
    {
        _enemyVision = GetComponent<EnemyVision>();
        _randomMovement = GetComponent<RandomMovementController>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<EnemyAnimator>();
        _suspicion = GetComponent<EnemySuspicionSystem>();
    }

    private void Start()
    {
        _currentState = AIState.Roam; // Initialize with roaming state
        _animator.SetAnimationTrigger(_currentState.ToString());
    }

    private void Update()
    {
        DetermineCurrentState();
        ApplyCurrentStateBehaviour();
        print(_currentState);
    }

    private void DetermineCurrentState()
    {
        _previousState = _currentState;
        if (_enemyVision.targetsLockedIn.Count > 0)
        {
            _currentState = AIState.Chase;
        }
        else if (_suspicion.GetSuspicionValue > 10)
        {
            _currentState = AIState.Patrol;
        }
        else
        {
            _currentState = AIState.Roam;
        }
    }

    private void ApplyCurrentStateBehaviour()
    {
        if(_previousState != _currentState)
        {
            _agent.ResetPath();
        }
        if (_currentState == AIState.Chase)
        {
            Transform target = _enemyVision.targetsLockedIn[0].transform;
            _agent.speed = chaseSpeed;
            _randomMovement.ChasePlayer(target);
        }
        else if (_currentState == AIState.Patrol)
        {
            Vector3 lastKnownPosition = _suspicion.lastSusPosition;
            _agent.speed = patrollingSpeed;
            _randomMovement.Patrol(lastKnownPosition);
        }
        else
        {
            _agent.speed = roamSpeed;
            _randomMovement.Roam();
        }
        
        //Set animation state based on current state (Animation triggers should be named exactly the same
        //as the current state's name for this to properly work. - Alden)
        _animator.SetAnimationTrigger(_currentState.ToString());
    }
}