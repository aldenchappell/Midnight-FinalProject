using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetMovment : MonoBehaviour
{
    [SerializeField] LayerMask nodeLayer;
    [SerializeField] float maxPatrolRange;
    [SerializeField] bool enableDebug;

    private Collider[] _allActiveNodes;
    private GameObject[] _allActiveDemonDoors;

    private NavMeshAgent _agent;
    private EnemySuspicionSystem _suspicion;

    private Vector3 _currentEndDestination;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _suspicion = GetComponent<EnemySuspicionSystem>();
    }

    private void Start()
    {
        _allActiveDemonDoors = GameObject.FindGameObjectsWithTag("DemonDoor");
    }

    public void SetCurrentMovementState(string state, Vector3 setPosition)
    {
       switch(state)
        {
            case "Roaming":
                CreateNewPatrolRoute();
                break;
            case "Chasing":
                SetChasePlayer(setPosition);
                break;
            case "Patrolling":
                PatrolArea(setPosition);
                break;
        }
    }

    

    private void SetChasePlayer(Vector3 target)
    {
        _agent.destination = target;
    }

    private void CreateNewPatrolRoute()
    { 
        if(_currentEndDestination != Vector3.zero)
        {
            print("Setting end");
            _agent.SetDestination(_currentEndDestination);
        }

        if (_agent.enabled == false || _currentEndDestination == Vector3.zero)
        {
            
            int randomStartIndex = Random.Range(0, _allActiveDemonDoors.Length);
            int randomEndIndex = Random.Range(0, _allActiveDemonDoors.Length);

            if (randomEndIndex == randomStartIndex)
            {
                if (randomEndIndex == _allActiveDemonDoors.Length - 1)
                {
                    randomEndIndex -= 1;
                }
                else
                {
                    randomEndIndex += 1;
                }
            }

            SetAIAtStartLocation(_allActiveDemonDoors[randomStartIndex]);
            _agent.enabled = true;
            _agent.SetDestination(_allActiveDemonDoors[randomEndIndex].transform.position);
            _currentEndDestination = _allActiveDemonDoors[randomEndIndex].transform.position;
        }
        else if (Vector3.Distance(transform.position, _currentEndDestination) <= _agent.stoppingDistance + 1)
        {
            _agent.enabled = false;
            _currentEndDestination = Vector3.zero;
            gameObject.SetActive(false);
        }

    }

    private void SetAIAtStartLocation(GameObject location)
    {
        print("Setting Position");
        gameObject.transform.position = location.transform.position;
    }

    private void PatrolArea(Vector3 patrolPositionCenter)
    {
        if (_agent.destination == transform.position)
        {
            Collider[] _allActiveNodes = Physics.OverlapSphere(patrolPositionCenter, maxPatrolRange, nodeLayer);

            int randomNodeIndex = Random.Range(0, _allActiveNodes.Length);
            _agent.destination = _allActiveNodes[randomNodeIndex].gameObject.transform.position;
        } 
        else if(_agent.remainingDistance <= _agent.stoppingDistance)
        {
            Collider[] _allActiveNodes = Physics.OverlapSphere(patrolPositionCenter, maxPatrolRange, nodeLayer);

            int randomNodeIndex = Random.Range(0, _allActiveNodes.Length);
            _agent.destination = _allActiveNodes[randomNodeIndex].gameObject.transform.position;
            _suspicion.PatrolNodeReached();
        }
    }

    private void OnDrawGizmos()
    {
        if (enableDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxPatrolRange);
        }
    }

}
