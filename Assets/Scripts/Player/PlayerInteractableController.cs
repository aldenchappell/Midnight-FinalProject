using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableController : MonoBehaviour
{
    
    private InteractableObject _interactableObject;
    
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private KeyCode interactionButton;
    
    [SerializeField] private Image interactionImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite defaultInteractionIcon;
    
    [SerializeField] private Vector2 defaultIconSize;
    [SerializeField] private Vector2 defaultInteractionIconSize;
    
    private void Update()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                2,
                interactableLayerMask))
        {
            if (hitInfo.collider.GetComponent<InteractableObject>() != false)
            {
                if(_interactableObject == null ||
                   _interactableObject.interactableID !=
                   hitInfo.collider.GetComponent<InteractableObject>().interactableID)
                {
                    _interactableObject = hitInfo.collider.GetComponent<InteractableObject>();
                    Debug.Log("Found interactable");
                }

                if (_interactableObject.interactionIcon != null)
                {
                    interactionImage.sprite = _interactableObject.interactionIcon;
                    if (_interactableObject.interactableIconSize == Vector2.zero)
                    {
                        interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                    }
                    else
                    {
                        interactionImage.rectTransform.sizeDelta = _interactableObject.interactableIconSize;
                    }
                }
                else
                {
                    interactionImage.sprite = defaultInteractionIcon;
                    interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                }

                if (Input.GetKeyDown(interactionButton))
                {
                    _interactableObject.onInteraction.Invoke();
                }
            }
        }
        else
        {
            if (interactionImage.sprite != defaultIcon)
            {
                interactionImage.sprite = defaultIcon;
                interactionImage.rectTransform.sizeDelta = defaultIconSize;
            }
        }
    }
}
