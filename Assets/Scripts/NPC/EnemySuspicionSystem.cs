using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuspicionSystem : MonoBehaviour
{
    [Header("Suspicion Values")]
    [Range(0, 40)]
    [SerializeField] float suspicionValue;

    EnemyVision _COV;

    private bool _isLosingSuspicion;

    private void Awake()
    {
        _COV = GetComponent<EnemyVision>();
    }

    private void Update()
    {
        if(_COV.GetRealizationValue >= 20)
        {
            suspicionValue = 40;
        }
    }

    public void AddSuspicion(int addedSuspicion)
    {
        suspicionValue += addedSuspicion;
        if(suspicionValue > 40)
        {
            suspicionValue = 40;
        }
    }

    private IEnumerator SuspicionLoss()
    {
        while(suspicionValue > 0)
        {
            suspicionValue -= 5;
            yield return new WaitForSeconds(1f);
        }
        
    }
}
