using UnityEngine;

public class MazeBallPrefab : MonoBehaviour
{
    private PlayerDualHandInventory _playerDualHandInventory;
    private void Awake()
    {
        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        var interactableObject = GetComponent<InteractableObject>();
        interactableObject.onInteraction.AddListener(() => _playerDualHandInventory.AdjustInventorySlots = gameObject);
    }
}
