using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuspicionSystem : MonoBehaviour
{
    [Header("Suspicion Values")]
    [Range(0, 40)]
    [SerializeField] float suspicionValue;

    EnemyVision _COV;

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

    }

    private IEnumerator SuspicionLoss()
    {
        yield return new WaitForSeconds(0f);
    }
}
