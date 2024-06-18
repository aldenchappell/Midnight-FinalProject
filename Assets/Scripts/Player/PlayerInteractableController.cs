using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableController : MonoBehaviour
{
    private InteractableObject _interactableObject;
    private InteractableObject _previousInteractable;
    private HighlightInteractableObjectController _highlightInteractableObjectController;
    private PlayerExamineObjectController _examineObjectController;
    private PauseManager _pauseManager;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite defaultInteractionIcon;
    [SerializeField] private Sprite examinationIcon;
    [SerializeField] private Sprite interactionCooldownIcon;
    [SerializeField] private Vector2 defaultIconSize;
    [SerializeField] private Vector2 defaultInteractionIconSize;
    [SerializeField] private float interactionDistance = 2.0f;
    [SerializeField] private float spamPreventionTime = 2.0f;
    private bool _allowInteraction = true;
    public bool _inPuzzle;

    private void Awake()
    {
        _examineObjectController = FindObjectOfType<PlayerExamineObjectController>();
        _pauseManager = FindObjectOfType<PauseManager>();
    }

    private void Update()
    {
        if (_examineObjectController != null && _examineObjectController.isExaminingObject) return; 
        if (!_allowInteraction) return;
        
        if (Physics.Raycast(
                mainCamera.transform.position,
                mainCamera.transform.forward,
                out var hitInfo,
                interactionDistance,
                interactableLayerMask))
        {
            var interactable = hitInfo.collider.GetComponent<InteractableObject>();
            var examinable = hitInfo.collider.GetComponent<ExaminableObject>();
            _examineObjectController.objectToExamine = hitInfo.collider.gameObject;
            if (interactable != null)
            {
                if (_interactableObject != interactable)
                {
                    ResetHighlight();

                    _interactableObject = interactable;

                    if (examinable != null)
                    {
                        interactionImage.sprite = examinationIcon;
                        interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                    }
                    else
                    {
                        UpdateInteractionUI(_interactableObject);
                    }

                    _highlightInteractableObjectController = _interactableObject.highlightInteractableObjectController;
                    _highlightInteractableObjectController?.ChangeColor(Color.red);
                }
            }
        }
        else
        {
            ResetHighlight();
            _examineObjectController.objectToExamine = null;
        }

        if (_interactableObject != _previousInteractable)
        {
            print("Hi");
            _previousInteractable = _interactableObject;
            if (_interactableObject != null && !_inPuzzle)
            {
                print("Bye");
                ResetInteraction();
            }
        }
        
        if ((Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKeyOne)
             || Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKeyTwo))
            && _examineObjectController != null
            && !_examineObjectController.isExaminingObject)
        {
            if (_interactableObject != null && _allowInteraction && _previousInteractable != null)
            {
                interactionImage.sprite = defaultInteractionIcon;
                interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                _interactableObject.onInteraction?.Invoke();
                StartCoroutine(InteractionSpamPrevention());
                _examineObjectController.objectToExamine = null;
            }
        }
        else if (Input.GetKeyDown(InGameSettingsManager.Instance.itemExaminationInteractionKey)
                 && _interactableObject != null
                 && _interactableObject.TryGetComponent<ExaminableObject>(out var examinableObject)
                 && !examinableObject.isExamining)
        {
            interactionImage.sprite = defaultInteractionIcon;
            interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
            _examineObjectController.StartExamination(examinableObject.gameObject);
            ResetInteraction(); 
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

    public bool IsLookingAtInteractableObject(GameObject target)
    {
        return _interactableObject != null && _interactableObject.gameObject == target;
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

    private IEnumerator InteractionSpamPrevention()
    {
        _allowInteraction = false;
        UpdateInteractionUI(_interactableObject);
        yield return new WaitForSeconds(spamPreventionTime);
        _allowInteraction = true;
        UpdateInteractionUI(_interactableObject);
    }

    private void ResetInteraction()
    {
        _interactableObject = null;
        UpdateInteractionUI(null);
        _highlightInteractableObjectController = null;

        DialogueController dialogueController = FindObjectOfType<DialogueController>();

        if (dialogueController != null && !_pauseManager.GameIsPaused)
        {
            dialogueController.ResetDialogue();
            dialogueController.DisableDialogueBox();
        
            //GlobalCursorManager.Instance.DisableCursor();
        }
    }
}
