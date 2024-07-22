using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoomMusic : MonoBehaviour
{
    // AudioSource component
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the same game object
        audioSource = GetComponent<AudioSource>();

        // Check if AudioSource component exists
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on " + gameObject.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Play the audio
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}


