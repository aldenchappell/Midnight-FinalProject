using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetMovment : MonoBehaviour
{
    [SerializeField] LayerMask nodeLayer;
    [SerializeField] float maxPatrolRange;

    private Collider[] _allActiveNodes;
    private GameObject[] _allActiveDemonDoors;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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
        print("Finding patrol route");
        if (_agent.enabled == false)
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
        }
        else if(_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _agent.enabled = false;
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
        if (_agent.destination == null || _agent.remainingDistance <= _agent.stoppingDistance)
        {
            Collider[] _allActiveNodes = Physics.OverlapSphere(patrolPositionCenter, maxPatrolRange, nodeLayer);

            int randomNodeIndex = Random.Range(0, _allActiveNodes.Length);
            _agent.destination = _allActiveNodes[randomNodeIndex].gameObject.transform.position;
        } 
    }

}
