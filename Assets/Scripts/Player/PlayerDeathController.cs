using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.SceneManagement;

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
        if (!other.CompareTag("Devil")) return;

        var enemy = other.GetComponent<EnemyStateController>();

        if (enemy != null && enemy.CheckForChaseState())
        {
            Die();

            enemy.ResetSuspicionValue();
            enemy.currentState = EnemyStateController.AIState.Roam;
        }
    }

    private void Die()
    {
        if (!isDead) // Prevent multiple deaths at the same time
        {
            // Reset puzzles on player death
            LevelCompletionManager.Instance.OnPlayerDeath();
            
            deathScreen.SetActive(true);
            isDead = true;
            StartCoroutine(OnDeathLoadDeathScreenRoutine());
        }
    }

    private IEnumerator OnDeathLoadDeathScreenRoutine()
    {
        firstPersonController.canMove = false;
        yield return new WaitForSeconds(5.0f); 

        ReturnToLobby();
    }

    private void ReturnToLobby()
    {
        //TO BE CHANGED TO LOBBY
        SceneManager.LoadScene("LOBBY");
    }
}
