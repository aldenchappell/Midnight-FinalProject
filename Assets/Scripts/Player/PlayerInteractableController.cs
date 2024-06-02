using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableController : MonoBehaviour
{
    private InteractableObject _interactableObject;
    private HighlightInteractableObjectController _highlightInteractableObjectController;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite defaultInteractionIcon;
    [SerializeField] private Vector2 defaultIconSize;
    [SerializeField] private Vector2 defaultInteractionIconSize;
    [SerializeField] private float interactionDistance = 2.0f;

    private bool _allowInteraction = true;

    private void Update()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out hitInfo,
            interactionDistance,
            interactableLayerMask))
        {
            var interactable = hitInfo.collider.GetComponent<InteractableObject>();
            var button = hitInfo.collider.GetComponent<Button>();
            if (interactable != null)
            {
                if (_interactableObject != interactable)
                {
                    // Reset previous highlight
                    ResetHighlight();

                    // Highlight new object
                    _interactableObject = interactable;
                    
                    // Update interaction UI
                    UpdateInteractionUI(_interactableObject);

                    if (button == null || !interactable.GetComponent<InteractableNPC>())
                    {
                        _highlightInteractableObjectController = _interactableObject.highlightInteractableObjectController;
                        _highlightInteractableObjectController?.ChangeColor(Color.red); // Added null check
                    }
                }
            }
        }
        else
        {
            // No object detected, reset previous highlight and disable dialogue if interacting with NPC
            ResetHighlight();
            
            //Ignore this comment, my IDE was screaming at me hehe 
            // ReSharper disable once Unity.NoNullPropagation
            DialogueController.Instance?.DisableDialogueBox(); // Added null check
        }

        // Handle interaction input
        if (_allowInteraction && Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKey))
        {
            if (_interactableObject != null)
            {
                if (_interactableObject is InteractableNPC interactableNPC)
                {
                    if (!DialogueController.Instance.dialogueEnabled)
                        interactableNPC.Interact();
                }
                else
                {
                    _interactableObject.onInteraction?.Invoke();
                }
            }
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
        if (interactionImage == null) return; // Added null check

        if (interactable != null && interactable.interactionIcon != null)
        {
            interactionImage.sprite = interactable.interactionIcon;
            interactionImage.rectTransform.sizeDelta =
                interactable.interactableIconSize == Vector2.zero ?
                    defaultInteractionIconSize : interactable.interactableIconSize;
        }
        else
        {
            interactionImage.sprite = defaultInteractionIcon;
            interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
        }
    }

    public bool IsLookingAtInteractableObject(GameObject target)
    {
        return _interactableObject != null && _interactableObject.gameObject == target;
    }
}
