using UnityEngine;

public class PolaroidPrefab : MonoBehaviour
{
    private PlayerDualHandInventory _playerDualHandInventory;
    private void Awake()
    {
        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        var interactableObject = GetComponent<InteractableObject>();
        interactableObject.onInteraction.AddListener(() => _playerDualHandInventory.AdjustInventorySlots = gameObject);
    }
}
