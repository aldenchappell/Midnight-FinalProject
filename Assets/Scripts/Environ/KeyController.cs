using UnityEngine;

public class KeyController : MonoBehaviour
{
    private InteractableObject _interactable;
    private PlayerKeyController _playerKeyController;
    
    private void Awake()
    {
        _interactable = GetComponent<InteractableObject>();
        
        _playerKeyController = FindObjectOfType<PlayerKeyController>();
        
        if (_interactable != null && _playerKeyController != null)
        {
            _interactable.onInteraction.AddListener(_playerKeyController.CollectKey);
        }
        else
        {
            Debug.LogError("InteractableObject or PlayerKeyController is missing");
        }
    }

    public void OnPickupDestroy()
    {
        Destroy(gameObject);
    }

    public void SetKeyActive()
    {
        gameObject.SetActive(true);
    }
}