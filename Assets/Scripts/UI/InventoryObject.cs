using UnityEngine;

public class InventoryObject : MonoBehaviour
{
    [SerializeField] private Sprite itemImage;
    public Sprite GetItemImage()
    {
        return itemImage;
    }
}
