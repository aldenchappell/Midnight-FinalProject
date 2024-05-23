using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class RandomMovementController : MonoBehaviour
{
    private NavMeshAgent _agent; 

    [Header("Randomized Movement Parameters")]
    [SerializeField] private float waypointSpawnRadius; // Radius for spawning waypoints.
    [SerializeField] private Transform centerPoint; // Center point of the map (e.g., ground or ground floor).
    [SerializeField] private float maxDistanceFromRandomPoint; // Max distance from a random point to a valid NavMesh position.

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>(); 
    }

    public void Roam()
    {
        Debug.Log("Roaming");
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            Vector3 destinationPoint;
            if (GetRandomWaypoint(centerPoint.position, waypointSpawnRadius, out destinationPoint))
            {
#if UNITY_EDITOR
                Debug.DrawRay(destinationPoint, Vector3.up, Color.red, maxDistanceFromRandomPoint); // Visualize the waypoint in the editor.
#endif
                _agent.SetDestination(destinationPoint); // Set the agent's destination to the random waypoint.
            }
        }
    }

    public void ChasePlayer(Transform player)
    {
        Debug.Log("Chasing");
        _agent.SetDestination(player.position);
    }

    public void Patrol(Vector3 lastKnownPosition)
    {
        Debug.Log("Patrolling");
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            Vector3 destinationPoint;
            if (GetRandomWaypoint(lastKnownPosition, waypointSpawnRadius, out destinationPoint))
            {
#if UNITY_EDITOR
                Debug.DrawRay(destinationPoint, Vector3.up, Color.blue, maxDistanceFromRandomPoint); // Visualize the waypoint in the editor.
#endif
                _agent.SetDestination(destinationPoint); // Set the agent's destination to the random waypoint near the last known position.
            }
        }
    }

    private bool GetRandomWaypoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomlyGeneratedPoint = center + Random.onUnitSphere * range; // Generate a random point around the center.
        if (NavMesh.SamplePosition(randomlyGeneratedPoint, out var hitInfo, 1.0f, NavMesh.AllAreas))
        {
            result = hitInfo.position; // Use the valid NavMesh position as the result.
            return true;
        }
        result = Vector3.zero;
        return false; // Unable to find a valid waypoint.
    }
}
