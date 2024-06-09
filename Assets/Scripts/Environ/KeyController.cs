using UnityEngine;

public class KeyController : MonoBehaviour
{
    private InteractableObject _interactable;
    private PlayerKeyController _playerKeyController;
    private ElevatorController _elevatorController;

    public bool isTaggedKey;
    public bool collected;
    private void Awake()
    {
        _interactable = GetComponent<InteractableObject>();
        
        _playerKeyController = FindObjectOfType<PlayerKeyController>();
        
        _elevatorController = FindObjectOfType<ElevatorController>();
        
        if (_interactable != null && _playerKeyController != null)
        {
            _interactable.onInteraction.AddListener(_elevatorController.OpenElevator);
            
            if(!isTaggedKey)
                _interactable.onInteraction.AddListener(_playerKeyController.CollectKey);
            
        }
        else
        {
            Debug.LogError("InteractableObject or PlayerKeyController is missing");
        }
    }

    public void OnPickupDestroy()
    {
        if (isTaggedKey)
        {
            collected = true;
        }
        
        Destroy(gameObject);
    }

    public void SetKeyActive()
    {
        gameObject.SetActive(true);
    }
}