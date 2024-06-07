using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static DialogueController;
using UnityEngine.InputSystem;

public class PlayerInteractableController : MonoBehaviour
{
    private InteractableObject _interactableObject;
    private HighlightInteractableObjectController _highlightInteractableObjectController;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite defaultInteractionIcon;
    [SerializeField] private Sprite interactionCooldownIcon;
    [SerializeField] private Vector2 defaultIconSize;
    [SerializeField] private Vector2 defaultInteractionIconSize;
    [SerializeField] private float interactionDistance = 2.0f;

    private const float SpamPreventionTime = 2.0f;
    private bool _allowInteraction = true;
    
    private GameObject _skullCompanion;
    private PlayerDualHandInventory _playerInventory;
    private void Awake()
    {
        _skullCompanion = GameObject.FindWithTag("Skull");
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>()
            .GetComponent<PlayerDualHandInventory>();
    }

    private void Update()
    {
        if (Physics.Raycast(
                mainCamera.transform.position,
                mainCamera.transform.forward,
                out var hitInfo,
                interactionDistance,
                interactableLayerMask))
        {
            var interactable = hitInfo.collider.GetComponent<InteractableObject>();
            var button = hitInfo.collider.GetComponent<Button>();
            if (interactable != null)
            {
                if (_interactableObject != interactable)
                {
                    ResetHighlight();
                    
                    _interactableObject = interactable;
                    
                    UpdateInteractionUI(_interactableObject);

                    if (button == null || !interactable.GetComponent<InteractableNPC>())
                    {
                        _highlightInteractableObjectController = _interactableObject.highlightInteractableObjectController;
                        _highlightInteractableObjectController?.ChangeColor(Color.red); 
                    }
                }
            }
        }
        else
        {
            ResetHighlight();
            
            if (_skullCompanion != null && !_skullCompanion.GetComponent<SkullDialogue>().pickedUp)
            {
                Instance.DisableDialogueBox(); 
            }
        }
        
        if (Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKey) && _interactableObject != null && _allowInteraction)
        {
            interactionImage.sprite = defaultInteractionIcon;
            interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
            if (_interactableObject is InteractableNPC interactableNPC)
            {
                if (_skullCompanion != null && _playerInventory.IsSkullInFirstSlot())
                {
                    interactableNPC.Interact();
                }
            }

            else
            {
                _interactableObject.onInteraction?.Invoke();
            }

            StartCoroutine(InteractionSpamPrevention());
        }


    }

    private void ResetHighlight()
    {
        if (_highlightInteractableObjectController != null)
        {
            _highlightInteractableObjectController.ResetColor();
            _highlightInteractableObjectController = null;
        }

        if (_interactableObject != null)
        {
            UpdateInteractionUI(null);
            _interactableObject = null;
        }
    }

    private void UpdateInteractionUI(InteractableObject interactable)
    {
        if (interactionImage == null) return;

        if (interactable != null && interactable.interactionIcon != null)
        {
            interactionImage.sprite = _allowInteraction ? interactable.interactionIcon : interactionCooldownIcon;
            interactionImage.rectTransform.sizeDelta =
                interactable.interactableIconSize == Vector2.zero ?
                    defaultInteractionIconSize : interactable.interactableIconSize;
        }
        else
        {
            interactionImage.sprite = _allowInteraction ? defaultIcon : defaultInteractionIcon;
            interactionImage.rectTransform.sizeDelta = defaultIconSize;
        }
    }


    public bool IsLookingAtInteractableObject(GameObject target)
    {
        return _interactableObject != null && _interactableObject.gameObject == target;
    }

    private IEnumerator InteractionSpamPrevention()
    {
        _allowInteraction = false;
        UpdateInteractionUI(_interactableObject);
        yield return new WaitForSeconds(SpamPreventionTime);
        _allowInteraction = true;
        UpdateInteractionUI(_interactableObject);
    }
}