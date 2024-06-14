using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//public class SuspicionNodeEvent: UnityEvent<Vector3>

public class EnemySuspicionSystem : MonoBehaviour
{
    //Unity Event to Add Suspicion
    //public SuspicionNodeEvent suspicionQue;

    [Header("Suspicion Values")]
    [Range(0, 40)]
    public float suspicionValue;

    //Vector3 of the last position of suspicious activity
    public Vector3 lastSusPosition;

    private EnemyVision _COV;

    //private bool _isLosingSuspicion;

    //Get Set Methods
    public float GetSuspicionValue
    {
        get
        {
            return suspicionValue;
        }
    }

    private void Awake()
    {
        _COV = GetComponent<EnemyVision>();

    }

    private void Start()
    {
        //Creating instance of suspicionQue Unity event
        /*
        if(suspicionQue == null)
        {
            suspicionQue = new SuspicionNodeEvent();
        }
        */
    }

    private void Update()
    {
        //If the demon is actively chasing the player, trigger suspicion continuously
        if(_COV.targetsLockedIn.Count > 0)
        {
            SuspicionTriggered(_COV.targetsLockedIn[0].transform.position);
        }
        //Press M to add suspicion manually (testing only)
        if(Input.GetKeyDown("m"))
        {
            SuspicionTriggered(GameObject.FindWithTag("Player").transform.position);
            print("Added Suspicion");
        }
    }
    //Adds suspicion and gets the position of where it occured
    public void SuspicionTriggered(Vector3 position)
    {
        lastSusPosition = position;
        AdjustSuspicionValue(10);
    }

    public void PatrolNodeReached()
    {
        AdjustSuspicionValue(-5);
    }
    
    private void AdjustSuspicionValue(int addedSuspicion)
    {
        suspicionValue += addedSuspicion;
        if(suspicionValue > 40)
        {
            suspicionValue = 40;
        }
        else if(suspicionValue < 0 )
        {
            suspicionValue = 0;
        }
    }
    

    
}
