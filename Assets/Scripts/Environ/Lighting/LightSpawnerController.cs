using System.Collections.Generic;
using UnityEngine;

public class LightSpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject lightPrefab;
    
    public Vector3 LightSpawnPosition(List<GameObject>  lights)
    {
        foreach (var light in lights)
        {
            return light.transform.position;
        }

        return Vector3.zero;
    }

    public void SpawnLights(Vector3[] positions)
    {
        foreach (var pos in positions)
        {
            Instantiate(lightPrefab, pos, Quaternion.identity);
        }
    }
}
