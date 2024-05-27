using System.Collections;
using UnityEngine;
using StarterAssets;

public class PlayerDeathController : MonoBehaviour
{
    public bool isDead = false;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Transform lobbySpawnPosition;
    [SerializeField] private FirstPersonController firstPersonController;

    private void Awake()
    {
        deathScreen.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Devil"))
        {
            var enemy = other.GetComponent<EnemyStateController>();

            if (enemy != null && enemy.CheckForChaseState())
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (!isDead) // Prevent multiple deaths at the same time
        {
            deathScreen.SetActive(true);
            isDead = true;
            StartCoroutine(OnDeathLoadDeathScreenRoutine());
        }
    }

    private IEnumerator OnDeathLoadDeathScreenRoutine()
    {
        firstPersonController.canMove = false;
        yield return new WaitForSeconds(5.0f); // Reduced delay

        ReturnToLobby();
    }

    private void ReturnToLobby()
    {
        // Move the player to the lobby spawn position
        transform.position = lobbySpawnPosition.position;

        deathScreen.SetActive(false); // Hide the death screen
        isDead = false; // Reset death status

        StartCoroutine(WaitToEnableMovement());
    }

    private IEnumerator WaitToEnableMovement()
    {
        yield return new WaitForSeconds(0.25f);
        firstPersonController.canMove = true; // Allow movement
    }
}
