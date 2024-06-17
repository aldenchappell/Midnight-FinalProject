using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolSystemManager : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] GameObject Demon;
    [SerializeField] bool hasFirstTimeSpawnCondition;

    private float _currentTime;

    public float DecreaseTimeToSpawn
    {
        set
        {
            if(!Demon.activeSelf)
            {
                _currentTime += value;
                print(_currentTime);
            }
        }
    }
    public Vector3 ReferenceToSuspicion
    {
        set
        {
            Demon.GetComponent<EnemySuspicionSystem>().SuspicionTriggered(value);
        }
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
            Demon.SetActive(true);
        }
    }

    public void ForceDemonSpawn()
    {
        _currentTime = 0;
        Demon.SetActive(true);
    }

    public void FirstTimeSpawn()
    {
        if (!Demon.activeSelf)
        {
            hasFirstTimeSpawnCondition = false;
            Demon.SetActive(true);
        }  
    }



    

}
