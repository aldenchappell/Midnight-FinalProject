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

        FindObjectOfType<MazeBallPuzzle>().marble = gameObject;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.name);
        _audio.PlayOneShot(dropSound);
        //Debug.Log("hit ground");
    }
}
