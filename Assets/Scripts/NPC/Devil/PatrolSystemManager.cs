using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolSystemManager : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] GameObject Demon;
    [SerializeField] GameObject pentAnim;
    [SerializeField] bool hasFirstTimeSpawnCondition;

    private float _currentTime;
    private GameObject[] _allActiveDemonDoors;


    public int DecreaseTimeToSpawn
    {
        set
        {
            if(!Demon.activeSelf)
            {
                _currentTime += value;
                print(_currentTime);
            }
            else
            {
                print("Raising");
                Demon.GetComponent<EnemySuspicionSystem>().SuspicionTriggered(GameObject.Find("Player").transform.position, value);
            }
        }
    }
    
    public int RaiseSuspicion
    {
        set
        {
            print("Raising");
            Demon.GetComponent<EnemySuspicionSystem>().SuspicionTriggered(GameObject.Find("Player").transform.position, value);
        }
    }

    private void Start()
    {
        _allActiveDemonDoors = GameObject.FindGameObjectsWithTag("DemonDoor");
    }


    private void Update()
    {
        if(!Demon.activeSelf && !hasFirstTimeSpawnCondition)
        {
            CheckTime();
        }
    }

    private void CheckTime()
    {
        _currentTime += Time.deltaTime;
        
        if(_currentTime >= timeBetweenSpawns)
        {
            _currentTime = 0;
            GenerateStartPoint();
        }
    }

    public void ForceDemonSpawn()
    {
        if(!Demon.activeSelf)
        {
            _currentTime = 0;
            GenerateStartPoint();
        }
    }

    public void FirstTimeSpawn()
    {
        if (!Demon.activeSelf)
        {
            hasFirstTimeSpawnCondition = false;
            GameObject demonAnim = Instantiate(pentAnim, Demon.GetComponent<SetMovment>().firstSpawn.transform.position, Quaternion.identity);
            demonAnim.GetComponent<Animator>().SetTrigger("Spawn");
            Destroy(demonAnim, 5.15f);
            Invoke("SetDemonActive", 5.15f);
        }  
    }

    private void GenerateStartPoint()
    {
        int randomStartIndex = Random.Range(0, _allActiveDemonDoors.Length);
        Demon.GetComponent<SetMovment>().spawnLocal = _allActiveDemonDoors[randomStartIndex];
        GameObject demonAnim = Instantiate(pentAnim, _allActiveDemonDoors[randomStartIndex].transform.position, Quaternion.identity);
        demonAnim.GetComponent<Animator>().SetTrigger("Spawn");
        Destroy(demonAnim, 5.15f);
        Invoke("SetDemonActive", 5.15f);
    }

    private void SetDemonActive()
    {
        Demon.SetActive(true);
    }



    

}
