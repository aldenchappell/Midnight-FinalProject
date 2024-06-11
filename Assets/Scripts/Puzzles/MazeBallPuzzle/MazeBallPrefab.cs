using UnityEngine;

public class MazeBallPrefab : MonoBehaviour
{
    private PlayerDualHandInventory _playerDualHandInventory;
    
    private AudioSource _audio;
    [SerializeField] private AudioClip dropSound;
    private void Awake()
    {
        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        var interactableObject = GetComponent<InteractableObject>();
        interactableObject.onInteraction.AddListener(() => _playerDualHandInventory.AdjustInventorySlots = gameObject);
        
        _audio = GetComponent<AudioSource>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        _audio.PlayOneShot(dropSound);
        Debug.Log("hit ground");
    }
}
