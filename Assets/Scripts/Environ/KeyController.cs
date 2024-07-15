using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyController : MonoBehaviour
{
    private InteractableObject _interactable;
    private PlayerProgressionController _playerProgressionController;
    private ElevatorController _elevatorController;

    public bool isTaggedKey;
    public bool collected;
    private void Awake()
    {
        _interactable = GetComponent<InteractableObject>();
        
        _playerProgressionController = FindObjectOfType<PlayerProgressionController>();
        
        _elevatorController = FindObjectOfType<ElevatorController>();
        
        if (_interactable != null && _playerProgressionController != null)
        {
            _interactable.onInteraction.AddListener(_elevatorController.OpenElevator);

            if (isTaggedKey)
            {
                _interactable.onInteraction.AddListener(() => _elevatorController.ToggleElevatorButtons());
                
                //fix for constantine saying return key when placing key in lobby
                if(SceneManager.GetActiveScene().name != "LOBBY")
                    _interactable.onInteraction.AddListener(_playerProgressionController.CollectKey);
                
            }
                

            if (isTaggedKey && SceneManager.GetActiveScene().name == "FLOORTHREE")
            {
                _interactable.onInteraction.AddListener(() => LevelCompletionManager.Instance.FinishGame());
                Debug.Log("Adding listener to finish the fuckin game");
            }
            
        }
        else
        {
            Debug.LogError("InteractableObject or PlayerKeyController is missing");
        }
    }

    private void Start()
    {
        if(isTaggedKey)
        {
            gameObject.SetActive(false);
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