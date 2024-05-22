using UnityEngine;
using UnityEngine.AI;

public class EnemyStateController : MonoBehaviour
{
    //Enemy State Parameters
    enum AIState { roaming, chasing, patrolling};
    AIState currentState;

    [Header("Movement Speed Values")]
    [SerializeField] float roamSpeed;
    [SerializeField] float chaseSpeed;

    //Script References
    EnemyVision COV;
    RandomMovementController RM;

    private void Start()
    {
        COV = GetComponent<EnemyVision>();
        RM = GetComponent<RandomMovementController>();
    }


    private void DetermineCurrentState()
    {
        if(COV.targetsInSight.Count > 0 && COV.realizationValue > 20)
        {
            currentState = AIState.chasing;
        }
        else if(COV.realizationValue > 0)
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
        if(currentState == AIState.chasing)
        {
            Transform target = COV.targetsInSight[0].transform;
            //Insert code here to adjust RandomMovement to chase player. Reference provided above
        }
        else if(currentState == AIState.patrolling)
        {
            Vector3 lastKnownPosition = COV.lastKnownPosition;
            //Insert code here to adjust RandomMovement to patrol around the players last known position, provided above
        }
        else
        {
            //Insert code here to reset AI movement to default roaming
        }
    }






    // public enum EnemyStates
    // {
    //     Roam,
    //     Chase
    // }
    //
    // //Speed of the enemy when they are roaming
    // public float roamSpeed = 2.0f;
    // //Speed of the enemy when they are chasing
    // public float chaseSpeed = 2.5f;
    //
    // public EnemyStates currentEnemyStates;
    //
    // public void DetermineEnemyState(Transform player)
    // {
    //     float distanceToPlayer = Vector3.Distance(player.position, transform.position);
    //
    //     switch (distanceToPlayer)
    //     {
    //         
    //     }
    // }
    //
    // public void DetermineEnemySpeedBasedOnEnemyState(NavMeshAgent agent, EnemyStates states)
    // {
    //     switch (states)
    //     {
    //         case EnemyStates.Roam: agent.speed = roamSpeed; break;
    //         case EnemyStates.Chase: agent.speed = chaseSpeed; break;
    //         default: agent.speed = roamSpeed; break;
    //     }
    // }
}
