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
    private SetMovment _setMovment;
    private NavMeshAgent _agent;
    private EnemyAnimator _animator;
    private EnemySuspicionSystem _suspicion;
    
    
    
    private void Awake()
    {
        _enemyVision = GetComponent<EnemyVision>();
        _setMovment = GetComponent<SetMovment>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<EnemyAnimator>();
        _suspicion = GetComponent<EnemySuspicionSystem>();
    }

    private void Start()
    {
        currentState = AIState.Roam; // Initialize with roaming state
        _animator.SetAnimationTrigger(currentState.ToString());
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
        else if (_suspicion.GetSuspicionValue > 0)
        {
            currentState = AIState.Patrol;
        }
        else
        {
            currentState = AIState.Roam;
        }
    }

    private void ApplyCurrentStateBehaviour()
    {
        if(_previousState != currentState)
        {
            _agent.ResetPath();
        }
        if (currentState == AIState.Chase)
        {
            Transform target = _enemyVision.targetsLockedIn[0].transform;
            _agent.speed = chaseSpeed;
            _setMovment.SetCurrentMovementState("Chasing", target.position);

            // if (_canAttack)
            //     Attack(target);
        }
        else if (currentState == AIState.Patrol)
        {
            Vector3 lastKnownPosition = _suspicion.lastSusPosition;
            _agent.speed = patrollingSpeed;
            _setMovment.SetCurrentMovementState("Patrolling", lastKnownPosition);

        }
        else
        {
            _agent.speed = roamSpeed;
            _setMovment.SetCurrentMovementState("Roaming", Vector3.zero);
        }
        
        //Set animation state based on current state (Animation triggers should be named exactly the same
        //as the current state's name for this to properly work. - Alden)
        _animator.SetAnimationTrigger(currentState.ToString());
    }

    public bool CheckForChaseState()
    {
        return currentState == AIState.Chase;
    }

    public void ResetSuspicionValue()
    {
        _suspicion.suspicionValue = 0;
    }
    
    // private void Attack(Transform target)
    // {
    //     if (Vector3.Distance(transform.position, target.position) < attackRange)
    //     {
    //        
    //         if (Physics.SphereCast(
    //                 attackPoint.position,
    //                 attackRadius,
    //                 attackPoint.forward,
    //                 out var hitInfo,
    //                 attackRange))
    //         {
    //             
    //             if (hitInfo.collider.CompareTag("Player"))
    //             {
    //                 
    //                 _animator.SetAnimationTrigger("Attack");
    //                 
    //                 var playerHealth = hitInfo.collider.GetComponentInParent<PlayerDeathController>();
    //                 
    //                 if (playerHealth != null)
    //                 {
    //                     playerHealth.Die();
    //                 }
    //             }
    //         }
    //
    //         StartCoroutine(AttackCooldown());
    //     }
    // }
    //
    // private IEnumerator AttackCooldown()
    // {
    //     _canAttack = false;
    //     yield return new WaitForSeconds(attackCooldownTimer);
    //     _canAttack = true;
    // }
}