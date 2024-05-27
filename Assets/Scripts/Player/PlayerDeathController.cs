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


    //Note from Owen: Change the functionality of return to lobby to swap the scene to "LOBBY"
    //If above is not what you want, see code below
    private void ReturnToLobby()
    {
        //Disable character controller component to allow teleport
        GetComponent<CharacterController>().enabled = false;

        // Move the player to the lobby spawn position
        transform.position = lobbySpawnPosition.position;

        //Renable character controller component to allow movement
        GetComponent<CharacterController>().enabled = true;
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
