using UnityEngine;
using UnityEngine.AI;

public class EnemyStateController : MonoBehaviour
{
    public enum EnemyStates
    {
        Roam,
        Chase
    }

    //Speed of the enemy when they are roaming
    public float roamSpeed = 2.0f;
    //Speed of the enemy when they are chasing
    public float chaseSpeed = 2.5f;
    
    public EnemyStates currentEnemyStates;

    public void DetermineEnemyState(Transform player)
    {
        
    }
    
    public void DetermineEnemySpeedBasedOnEnemyState(NavMeshAgent agent, EnemyStates states)
    {
        switch (states)
        {
            case EnemyStates.Roam: agent.speed = roamSpeed; break;
            case EnemyStates.Chase: agent.speed = chaseSpeed; break;
            default: agent.speed = roamSpeed; break;
        }
    }
}
