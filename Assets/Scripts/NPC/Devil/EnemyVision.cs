using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    //Parameters that dictate search radius and cone of vision field
    [Header("Search Parameters")]
    [SerializeField] float searchRadius;
    [Range(0,360)]
    [SerializeField] float coneRadius;
    
    //Layer of the target AI is searching for
    [Header("Target Layer")]
    [SerializeField] LayerMask targetLayer;

    //Enable this in inspector to see debug information for COV
    [Header("Enable Debug Statements")]
    [SerializeField] bool enableDebug;

    //List of all targets that are in range, sight, and not obstructed from view.
    private List<GameObject> targetsInSight = new List<GameObject>(1);
    public List<GameObject> targetsLockedIn = new List<GameObject>(1);
    public Vector3 lastKnownPosition;

    //Parameters that adjust AI's realization values
    [Header("Realization Function Values")]
    [Range(0, 20)]
    [SerializeField] float realizationValue;
    bool isRealizing;

    //Get Set Methods
    public float GetRealizationValue
    {
        get
        {
            return realizationValue;
        }
    }


    private EnemyStateController _stateController;

    private void Awake()
    {
        _stateController = GetComponent<EnemyStateController>();
    }

    private void Update()
    {
        GetTargetsInRadius();
        if(targetsInSight.Count > 0 && !isRealizing)
        {
            Invoke("RealiziationBuffer", .05f);
            isRealizing = true;
        }
        if(targetsLockedIn.Count > 0)
        {
            HasTargetHid();
        }
        
    }

    //Get all targets in radius and determine if they are visible to AI
    private void GetTargetsInRadius()
    {
        targetsInSight.Clear();
        Collider[] targets = Physics.OverlapSphere(transform.position, searchRadius, targetLayer);
        if(targets.Length > 0)
        {
            foreach(Collider target in targets)
            {
                Vector3 targetDir = (target.transform.position - transform.position);
                float angle = Vector3.Angle(targetDir, transform.forward);
                if(Mathf.Abs(angle) < coneRadius)
                {
                    if(Physics.Raycast(transform.position, targetDir, out RaycastHit hit, 100f))
                    {
                        if(hit.collider == target)
                        {
                            if(targetsInSight.Contains(target.gameObject) != true)
                            {
                                targetsInSight.Add(target.transform.gameObject);
                                lastKnownPosition = target.transform.position;
                                //print("Target " + target.name + " is in sight");
                            }   
                        }
                        else
                        {
                            if(enableDebug)
                            {
                                //print("Debug - EnemyVision(GetTargetsInRadius): " + target.name + " Is Obstructed From View");
                            }
                        }
                    }
                }
                else
                {
                    if(enableDebug)
                    {
                        //print("Debug - EnemyVision(GetTargetsInRadius): " + target.name + " Outside Enemy View");
                    }
                }
            }
        }
        else
        {
            if(enableDebug)
            {
               // print("Debug - EnemyVision(GetTargetsInRadius): No Targets In Radius");
            }
        }
    }

    //Raises and lowers Realization Value depending on if targets are in sight. Used for chasing and patrolling states in Enemy state Controller
    private void RealiziationBuffer()
    {
        if(targetsInSight.Count > 0)
        {
            realizationValue += 1; 
            //Target has been fully realized and is now being chased.
            if(realizationValue >= 20)
            {
                if(targetsLockedIn.Contains(targetsInSight[0]) != true)
                {
                    targetsLockedIn.Add(targetsInSight[0]);
                }
                realizationValue = 20;
            }
            Invoke("RealiziationBuffer", .05f);
        }
        else if(targetsInSight.Count <= 0 && realizationValue > 0)
        {
            realizationValue -= 2;
            Invoke("RealiziationBuffer", 1f);
        }
        else
        {
            //Target is no longer in sight and has been unrealized. 
            targetsLockedIn.Clear();
            isRealizing = false;
        }
    }

    private void HasTargetHid()
    {
        foreach(GameObject target in targetsLockedIn)
        {
            if(target.layer != LayerMask.NameToLayer("Target") && target != null)
            {
                print("Unrealizing");
                realizationValue = 0;
            }
        }
    }

    //Debug Gizmos
    // private void OnDrawGizmos()
    // {
    //     if(enableDebug)
    //     {
    //         Gizmos.DrawWireSphere(transform.position, searchRadius);
    //         Vector3 line1 = new Vector3(Mathf.Sin((coneRadius/2) * Mathf.Deg2Rad), 0, Mathf.Cos(coneRadius * Mathf.Deg2Rad));
    //         Vector3 line2 = new Vector3(Mathf.Sin((-coneRadius/2) * Mathf.Deg2Rad), 0, Mathf.Cos(coneRadius * Mathf.Deg2Rad));
    //         Gizmos.DrawLine(transform.position, transform.position + line1 * searchRadius);
    //         Gizmos.DrawLine(transform.position, transform.position + line2 * searchRadius);
    //         
    //     }
    // }
}
