using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableController : MonoBehaviour
{
    private InteractableObject _interactableObject;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite defaultInteractionIcon;
    [SerializeField] private Vector2 defaultIconSize;
    [SerializeField] private Vector2 defaultInteractionIconSize;
    [SerializeField] private float interactionDistance = 2.0f;
    
    
    //Spam prevention
    private bool _allowInteraction = true;
    private const float InteractionSpamPreventionTimer = 2.0f;
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
            if (interactable != null)
            {
                if (_interactableObject == null || _interactableObject.interactableID != interactable.interactableID)
                {
                    _interactableObject = interactable;
                }

                if (_interactableObject.interactionIcon != null)
                {
                    interactionImage.sprite = _interactableObject.interactionIcon;
                    
                    interactionImage.rectTransform.sizeDelta =
                    _interactableObject.interactableIconSize == Vector2.zero ? defaultInteractionIconSize
                        : _interactableObject.interactableIconSize;
                }
                else
                {
                    interactionImage.sprite = defaultInteractionIcon;
                    interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                }

                if (_allowInteraction && Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKey))
                {
                    if (_interactableObject is InteractableNPC interactableNPC)
                    {
                        if(!DialogueController.Instance.dialogueEnabled)
                            interactableNPC.Interact();
                    }
                    else
                    {
                        _interactableObject.onInteraction.Invoke();
                    }
                }
            }
        }
        else
        {
            if (interactionImage.sprite != defaultIcon)
            {
                interactionImage.sprite = defaultIcon;
                interactionImage.rectTransform.sizeDelta = defaultIconSize;
                DialogueController.Instance.DisableDialogueBox();
            }
        }
    }

    private IEnumerator PreventInteractionSpam()
    {
        _allowInteraction = false;
        yield return new WaitForSeconds(InteractionSpamPreventionTimer);
        _allowInteraction = true;
    }
}