using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetMovment : MonoBehaviour
{
    [SerializeField] LayerMask nodeLayer;
    [SerializeField] float maxPatrolRange;
    [SerializeField] bool enableDebug;
    [SerializeField] GameObject animPref;


    [Header("Only Assign if AI has special first time spawn event (Level One Only)")]
    [SerializeField] GameObject firstSpawn;
    [SerializeField] GameObject firstEnd;
    [SerializeField] bool hasFirstTimeSpawnCondition;

    private Collider[] _allActiveNodes;
    private GameObject[] _allActiveDemonDoors;
    private GameObject _player;

    private NavMeshAgent _agent;
    private EnemySuspicionSystem _suspicion;
    private PatrolSystemManager _patrolManager;

    private Vector3 _currentEndDestination;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _suspicion = GetComponent<EnemySuspicionSystem>();
        _player = GameObject.FindWithTag("Player");
        _patrolManager = GameObject.FindObjectOfType<PatrolSystemManager>();
    }

    private void Start()
    {
        _allActiveDemonDoors = GameObject.FindGameObjectsWithTag("DemonDoor");
    }

    public void SetCurrentMovementState(string state, Vector3 setPosition)
    {
        if (hasFirstTimeSpawnCondition)
        {
            _patrolManager.RaiseSuspicion = -40;
            state = "Roaming";
        }
        switch (state)
        {
            case "Roaming":
                CreateNewPatrolRoute();
                break;
            case "Chasing":
                if(_agent.enabled != true)
                {
                    _agent.enabled = true;
                }
                SetChasePlayer(setPosition);
                break;
            case "Patrolling":
                if (_agent.enabled != true)
                {
                    _agent.enabled = true;
                }
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
        if(hasFirstTimeSpawnCondition)
        {
            hasFirstTimeSpawnCondition = false;

            _agent.enabled = false;
            _currentEndDestination = firstEnd.transform.position;
            GameObject newAnim = Instantiate(animPref, firstSpawn.transform.position, Quaternion.identity);
            newAnim.GetComponent<Animator>().SetTrigger("Spawn");
            Destroy(newAnim, 5.15f);
            StartCoroutine(SetAIRoute(firstSpawn, firstEnd));

        }

        if(_currentEndDestination != Vector3.zero)
        {
            //print("Setting end");
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
            if(Vector3.Distance(_allActiveDemonDoors[randomStartIndex].transform.position, _player.transform.position) <= 10)
            {
                return;
            }
            else
            {
                _agent.enabled = false;
                _currentEndDestination = _allActiveDemonDoors[randomEndIndex].transform.position;
                GameObject newAnim = Instantiate(animPref, _allActiveDemonDoors[randomStartIndex].transform.position, Quaternion.identity);
                newAnim.GetComponent<Animator>().SetTrigger("Spawn");
                Destroy(newAnim, 5.15f);
                StartCoroutine(SetAIRoute(_allActiveDemonDoors[randomStartIndex], _allActiveDemonDoors[randomEndIndex]));

            }

            
        }
        else if (Vector3.Distance(transform.position, _currentEndDestination) <= _agent.stoppingDistance + 1)
        {
            _currentEndDestination = Vector3.zero;
            GameObject newAnim = Instantiate(animPref, transform.position, transform.rotation);
            newAnim.GetComponent<Animator>().SetTrigger("Despawn");
            Destroy(newAnim, .28f);
            gameObject.SetActive(false);
        }

    }

    private IEnumerator SetAIRoute(GameObject start, GameObject end)
    {
        yield return new WaitForSeconds(5.15f);
        //print("Setting Position");
        gameObject.transform.position = start.transform.position;
        _agent.enabled = enabled;
        _agent.SetDestination(end.transform.position);
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

    private void SpecialFirstPatrolRoute()
    {

    }

}
