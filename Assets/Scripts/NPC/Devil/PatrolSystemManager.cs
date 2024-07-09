using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PatrolSystemManager : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] GameObject Demon;
    [SerializeField] bool hasFirstTimeSpawnCondition;

    private float _currentTime;

    private PlayerCameraShake _shakeCam;

    private void Awake()
    {
        _shakeCam = FindObjectOfType<PlayerCameraShake>();
    }

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
            SetDemonActive();
        }
    }

    public void ForceDemonSpawn()
    {
        if(!Demon.activeSelf)
        {
            _currentTime = 0;
            SetDemonActive();
        }
    }

    private void SetDemonActive()
    {
        hasFirstTimeSpawnCondition = false;
        Demon.transform.GetChild(0).gameObject.SetActive(false);
        Demon.SetActive(true);
        
        Debug.Log("Shaking player camera");
        Demon.GetComponent<EnemyVision>().enabled = false;
        if (_shakeCam != null)
            _shakeCam.TriggerShake();
        
        // FindObjectOfType<PlayerHeartbeatController>().enemyStateController = GetComponent<EnemyStateController>();
        //FindObjectOfType<PlayerHeartbeatController>().shouldCheckHeartbeat = true;
    }
}
