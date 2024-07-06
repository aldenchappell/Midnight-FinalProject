using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetMovment : MonoBehaviour
{
    [SerializeField] LayerMask nodeLayer;
    [SerializeField] float maxPatrolRange;
    [SerializeField] bool enableDebug;
    [SerializeField] GameObject pentAnim;

    [Header("Only Assign if AI has special first time spawn event (Level One Only)")]
    public GameObject firstSpawn;
    [SerializeField] GameObject firstEnd;
    [SerializeField] bool hasFirstTimeSpawnCondition;

    public GameObject spawnLocal;

    private Collider[] _allActiveNodes;
    private GameObject[] _allActiveDemonDoors;
    private GameObject _player;

    private NavMeshAgent _agent;
    private EnemySuspicionSystem _suspicion;

    private Vector3 _currentEndDestination;
    private bool _isPreppingToSpawn;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _suspicion = GetComponent<EnemySuspicionSystem>();
        _player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        _allActiveDemonDoors = GameObject.FindGameObjectsWithTag("DemonDoor");
        _currentEndDestination = Vector3.zero;
    }

    public void SetCurrentMovementState(string state, Vector3 setPosition)
    {
        
       if(!_isPreppingToSpawn)
       {
            switch (state)
            {
                case "Roaming":
                    CreateNewPatrolRoute();
                    break;
                case "Chasing":
                    if (_agent.enabled != true)
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
            _isPreppingToSpawn = true;

            _agent.enabled = false;
            SetAIAtStartLocation(firstSpawn);
            _currentEndDestination = firstEnd.transform.position;
            GameObject demonAnim = Instantiate(pentAnim, firstSpawn.transform.position, Quaternion.identity);
            demonAnim.GetComponent<Animator>().SetTrigger("Spawn");
            Destroy(demonAnim, 5.15f);
            Invoke("GoForwardChild", 5.15f);
        }

        else if (_agent.enabled == false || _currentEndDestination == Vector3.zero)
        {
            if(spawnLocal != null)
            {
                int randomEndIndex = Random.Range(0, _allActiveDemonDoors.Length);
                if (_allActiveDemonDoors[randomEndIndex] == spawnLocal)
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
                _agent.enabled = false;
                SetAIAtStartLocation(spawnLocal);
                _agent.enabled = enabled;
                _currentEndDestination = _allActiveDemonDoors[randomEndIndex].transform.position;
                _agent.SetDestination(_allActiveDemonDoors[randomEndIndex].transform.position);

            }
            else
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
                if (Vector3.Distance(_allActiveDemonDoors[randomStartIndex].transform.position, _player.transform.position) <= 10)
                {
                    return;
                }
                else
                {
                    _isPreppingToSpawn = true;
                    _agent.enabled = false;
                    SetAIAtStartLocation(_allActiveDemonDoors[randomStartIndex]);
                    _currentEndDestination = _allActiveDemonDoors[randomEndIndex].transform.position;
                    GameObject demonAnim = Instantiate(pentAnim, _allActiveDemonDoors[randomStartIndex].transform.position, Quaternion.identity);
                    demonAnim.GetComponent<Animator>().SetTrigger("Spawn");
                    Destroy(demonAnim, 5.15f);
                    Invoke("GoForwardChild", 5.15f);
                }
            }
            

            
        }
        else if (Vector3.Distance(transform.position, _currentEndDestination) <= _agent.stoppingDistance + 1)
        {
            _currentEndDestination = Vector3.zero;
            GameObject demonAnim = Instantiate(pentAnim, transform.position, Quaternion.identity);
            demonAnim.GetComponent<Animator>().SetTrigger("Despawn");
            Destroy(demonAnim, 5.15f);
            gameObject.SetActive(false);
        }
        else
        {
            _agent.SetDestination(_currentEndDestination);
        }

    }

    private void GoForwardChild()
    {
        _isPreppingToSpawn = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        _agent.enabled = enabled;
        _agent.SetDestination(_currentEndDestination);
    }

    private void SetAIAtStartLocation(GameObject location)
    {
        //print("Setting Position");
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

    private void SpecialFirstPatrolRoute()
    {

    }

}
