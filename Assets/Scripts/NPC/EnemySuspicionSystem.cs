using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuspicionNodeEvent: UnityEvent<Vector3>
{

}
public class EnemySuspicionSystem : MonoBehaviour
{
    public SuspicionNodeEvent suspicionQue;

    [Header("Suspicion Values")]
    [Range(0, 40)]
    [SerializeField] float suspicionValue;

    public Vector3 lastSusPosition;

    private EnemyVision _COV;

    private bool _isLosingSuspicion;

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
        if(suspicionQue == null)
        {
            suspicionQue = new SuspicionNodeEvent();
        }
    }

    private void Update()
    {
        if(_COV.targetsLockedIn.Count > 0)
        {
            SuspicionTriggered(_COV.targetsLockedIn[0].transform.position);
        }
        if(Input.GetKey("m"))
        {
            SuspicionTriggered(GameObject.FindWithTag("Player").transform.position);
            print("Added Suspicion");
        }
    }

    public void SuspicionTriggered(Vector3 position)
    {
        lastSusPosition = position;
        AddSuspicion(10);
    }
    
    private void AddSuspicion(int addedSuspicion)
    {
        CancelInvoke();
        suspicionValue += addedSuspicion;
        if(suspicionValue > 40)
        {
            suspicionValue = 40;
        }
        Invoke("DelayToSuspicionLoss", 5f);
    }

    private IEnumerator SuspicionLoss()
    {
        while(suspicionValue > 0)
        {
            suspicionValue -= 5;
            yield return new WaitForSeconds(1f);
        }
        
    }
    private void DelayToSuspicionLoss()
    {
        StartCoroutine(SuspicionLoss());
    }

    
}
