using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerDeathController : MonoBehaviour
{
    public bool isDead = false;

    [HideInInspector] public bool isDying = false;

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] CinemachineVirtualCamera _playerCam;

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
            isDying = true;
            Die();

            enemy.ResetSuspicionValue();
            enemy.currentState = EnemyStateController.AIState.Roam;
        }
    }

    private void Die()
    {
        if (!isDead) 
        {
            LevelCompletionManager.Instance.UpdateKeyCount(0);
            LevelCompletionManager.Instance.hasKey = false;
            GameObject playerUI = GameObject.Find("INGAMEUI");
            
            if(playerUI != null)
            {
                playerUI.SetActive(false);
            }
            
            _playerCam.Priority = 20;
            // Reset puzzles on player death
            LevelCompletionManager.Instance.OnPlayerDeath();
            deathScreen.transform.parent.gameObject.SetActive(true);
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
