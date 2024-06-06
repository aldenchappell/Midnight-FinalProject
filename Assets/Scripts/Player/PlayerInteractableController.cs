using System;
using System.Collections;
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
    [SerializeField] private Sprite interactionCooldownIcon;
    [SerializeField] private Vector2 defaultIconSize;
    [SerializeField] private Vector2 defaultInteractionIconSize;
    [SerializeField] private float interactionDistance = 2.0f;

    private const float SpamPreventionTime = 2.0f;
    private bool _allowInteraction = true;
    
    private GameObject _skullCompanion;

    private void Awake()
    {
        _skullCompanion = GameObject.FindWithTag("Skull");
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
                    // Reset last highlight
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
            // No object detected, reset previous highlight
            ResetHighlight();

            // Disable dialogue only if the skull is not active
            if (_skullCompanion != null && !_skullCompanion.GetComponent<SkullDialogue>().pickedUp)
            {
                DialogueController.Instance?.DisableDialogueBox(); // Added null check
            }
        }

        // Handle interaction input
        if (_allowInteraction && Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKey))
        {
            if (_interactableObject != null && _allowInteraction)
            {
                interactionImage.sprite = defaultInteractionIcon;
                interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                if (_interactableObject is InteractableNPC interactableNPC)
                {
                    // Block interaction with NPCs if the skull is active
                    if (_skullCompanion != null && !_skullCompanion.activeSelf)
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
        UpdateInteractionUI(_interactableObject); // Update UI to show cooldown icon or default interaction icon if not looking at an object
        yield return new WaitForSeconds(SpamPreventionTime);
        _allowInteraction = true;
        UpdateInteractionUI(_interactableObject); // Update UI back to normal icon
    }
}