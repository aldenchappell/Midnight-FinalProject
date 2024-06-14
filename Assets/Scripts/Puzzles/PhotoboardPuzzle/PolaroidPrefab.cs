using UnityEngine;

public class PolaroidPrefab : MonoBehaviour
{
    private PlayerDualHandInventory _playerDualHandInventory;
    private Polaroid _polaroid;
    private void Awake()
    {
        _polaroid = GetComponent<Polaroid>();
        _playerDualHandInventory = FindObjectOfType<PlayerDualHandInventory>();
        var interactableObject = GetComponent<InteractableObject>();
        interactableObject.onInteraction.AddListener(() => _playerDualHandInventory.AdjustInventorySlots = gameObject);
        interactableObject.onInteraction.AddListener(() => _polaroid.SetPhotoBoardPieceAlpha());
    }
}
