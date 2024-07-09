using UnityEngine;

public class EnemyShakeCamera : MonoBehaviour
{
    private PlayerCameraShake _shakeCam;

    private void Awake()
    {
        _shakeCam = FindObjectOfType<PlayerCameraShake>();

        if (_shakeCam == null)
        {
            Debug.LogWarning("PlayerCameraShake script not found. Attempting to find by tag.");

            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            if (mainCamera != null)
            {
                _shakeCam = mainCamera.GetComponent<PlayerCameraShake>();
            }
            else
            {
                Debug.LogError("Main camera not found.");
            }
        }
    }

    public void ShakeCam()
    {
        if (_shakeCam != null)
        {
            _shakeCam.TriggerShake();
            Debug.Log("Shaking player camera");
        }
        else
        {
            Debug.LogWarning("PlayerCameraShake reference is null. ShakeCam method cannot be called.");
        }
    }
}