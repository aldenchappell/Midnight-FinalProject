using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateController : MonoBehaviour
{
    // Enemy State Parameters
    enum AIState { roaming, chasing, patrolling };
    AIState currentState;

    [Header("Movement Speed Values")]
    [SerializeField] private float roamSpeed = 1.85f;
    [SerializeField] private float chaseSpeed = 2.25f;
    [SerializeField] private float patrollingSpeed = 2.0f;

    // Script References
    EnemyVision COV;
    RandomMovementController RM;
    NavMeshAgent agent;

    private void Start()
    {
        COV = GetComponent<EnemyVision>();
        RM = GetComponent<RandomMovementController>();
        agent = GetComponent<NavMeshAgent>();
        currentState = AIState.roaming; // Initialize with roaming state
    }

    private void Update()
    {
        DetermineCurrentState();
        ApplyCurrentStateBehaviour();
    }

    private void DetermineCurrentState()
    {
        if (COV.targetsInSight.Count > 0 && COV.realizationValue > 20)
        {
            currentState = AIState.chasing;
        }
        else if (COV.realizationValue > 0)
        {
            currentState = AIState.patrolling;
        }
        else
        {
            currentState = AIState.roaming;
        }
    }

    private void ApplyCurrentStateBehaviour()
    {
        if (currentState == AIState.chasing)
        {
            Transform target = COV.targetsInSight[0].transform;
            agent.speed = chaseSpeed;
            RM.ChasePlayer(target);
        }
        else if (currentState == AIState.patrolling)
        {
            Vector3 lastKnownPosition = COV.lastKnownPosition;
            agent.speed = patrollingSpeed;
            RM.Patrol(lastKnownPosition);
        }
        else
        {
            agent.speed = roamSpeed;
            RM.Roam();
        }
    }
}