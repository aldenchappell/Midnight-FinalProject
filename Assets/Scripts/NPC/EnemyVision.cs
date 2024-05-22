using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Search Parameters")]
    [SerializeField] float searchRadius;
    [Range(0,360)]
    [SerializeField] float coneRadius;

    [Header("Target Layer")]
    [SerializeField] LayerMask targetLayer;

    [Header("Enable Debug Statements")]
    [SerializeField] bool enableDebug;

    //Found Targets
    List<GameObject> targetsInSight = new List<GameObject>();

    [Header("Realization Function Values")]
    [Range(0, 20)]
    [SerializeField] float realizationValue;
    bool isRealizing;


    private void Update()
    {
        GetTargetsInRadius();
        if(targetsInSight.Count > 0 && !isRealizing)
        {
            Invoke("RealiziationBuffer", .05f);
            isRealizing = true;
        }
    }


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
                            targetsInSight.Add(target.transform.gameObject);
                            print("Target " + target.name + " is in sight");
                        }
                        else
                        {
                            if(enableDebug)
                            {
                                print("Debug - EnemyVision(GetTargetsInRadius): " + target.name + " Is Obstructed From View");
                            }
                        }
                    }
                }
                else
                {
                    if(enableDebug)
                    {
                        print("Debug - EnemyVision(GetTargetsInRadius): " + target.name + " Outside Enemy View");
                    }
                }
            }
        }
        else
        {
            if(enableDebug)
            {
                print("Debug - EnemyVision(GetTargetsInRadius): No Targets In Radius");
            }
        }
    }

    private void RealiziationBuffer()
    {
        if(targetsInSight.Count > 0)
        {
            realizationValue += 1;
            Invoke("RealiziationBuffer", .05f);
        }
        else
        {
            realizationValue = 0;
            isRealizing = false;
        }
    }


    private void OnDrawGizmos()
    {
        if(enableDebug)
        {
            Gizmos.DrawWireSphere(transform.position, searchRadius);
            Vector3 line1 = new Vector3(Mathf.Sin((coneRadius/2) * Mathf.Deg2Rad), 0, Mathf.Cos(coneRadius * Mathf.Deg2Rad));
            Vector3 line2 = new Vector3(Mathf.Sin((-coneRadius/2) * Mathf.Deg2Rad), 0, Mathf.Cos(coneRadius * Mathf.Deg2Rad));
            Gizmos.DrawLine(transform.position, transform.position + line1 * searchRadius);
            Gizmos.DrawLine(transform.position, transform.position + line2 * searchRadius);
            
        }
    }
}
