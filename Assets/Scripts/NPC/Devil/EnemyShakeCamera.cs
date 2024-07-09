using UnityEngine;

public class EnemyShakeCamera : MonoBehaviour
{
    private PlayerCameraShake _shakeCam;
    
    private void Awake()
    {
        _shakeCam = FindObjectOfType<PlayerCameraShake>();

        if (_shakeCam == null)
        {
            _shakeCam = GameObject.FindWithTag("MainCamera").GetComponent<PlayerCameraShake>();
        }
    }

    public void ShakeCam()
    {
        if (_shakeCam != null)
        {
            _shakeCam.TriggerShake();
            Debug.LogError("Shaking player camera");
        }
    }
}
