using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private void Start()
    {
        ShakeCamera();
    }

    private void ShakeCamera()
    {
        StartCoroutine(Shake());
    }
    
    private IEnumerator Shake()
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsedTime = 0f;
        float duration = 20.25f;
        float magnitude = 2.5f;
            
        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        transform.localPosition = originalPosition;
    }
}
