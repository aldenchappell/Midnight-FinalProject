using System.Collections.Generic;
using UnityEngine;

public class EnemyFlickerLightsInVicinity : MonoBehaviour
{
    [SerializeField] private float radius = 10.0f;
    private List<LightFlicker> _currentFlickeringLights = new List<LightFlicker>();

    private void Update()
    {
        HandleFlicker();
    }

    private void HandleFlicker()
    {
        Collider[] lightColliders = Physics.OverlapSphere(transform.position, radius);
        List<LightFlicker> flickeringLights = new List<LightFlicker>();

        bool soundPlayed = false;

        foreach (var light in lightColliders)
        {
            LightFlicker[] flickerComponents = light.GetComponentsInChildren<LightFlicker>();
            foreach (var flicker in flickerComponents)
            {
                flickeringLights.Add(flicker);
                if (!_currentFlickeringLights.Contains(flicker))
                {
                    flicker.shouldFlicker = true;
                    flicker.StartFlickering();

                    if (!soundPlayed)
                    {
                        if (LightFlicker.AudioSource != null)
                        {
                            LightFlicker.AudioSource.transform.position = flicker.transform.position;
                            LightFlicker.AudioSource.PlayOneShot(flicker.flickerSound);
                            soundPlayed = true;
                        }
                    }
                }
            }
        }

        foreach (var flicker in _currentFlickeringLights)
        {
            if (!flickeringLights.Contains(flicker))
            {
                flicker.shouldFlicker = false;
                flicker.StopFlickering();
            }
        }

        _currentFlickeringLights = flickeringLights;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
