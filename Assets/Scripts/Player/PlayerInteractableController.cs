using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableController : MonoBehaviour
{
    [HideInInspector] public InteractableObject interactableObject;
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

    private PlayerDualHandInventory _playerInventory;
    private PlayerArmsAnimationController _playerArms;

    private void Awake()
    {
        _examineObjectController = FindObjectOfType<PlayerExamineObjectController>();
        _pauseManager = FindObjectOfType<PauseManager>();
        _playerArms = FindObjectOfType<PlayerArmsAnimationController>();
        _playerInventory = FindObjectOfType<PlayerDualHandInventory>();
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
                
                _playerArms.SetPickingUp(true);
                if(_playerInventory.GetInventory.Length > 0 && !interactable.GetComponent<Puzzle>())
                    _playerInventory.HideHandItem();
                
                if (interactableObject != interactable)
                {
                    ResetHighlight();
                    interactableObject = interactable;
                    _playerInventory.ShowCurrentIndexItem();
                    if (examinable != null)
                    {
                        interactionImage.sprite = examinationIcon;
                        interactionImage.rectTransform.sizeDelta = defaultInteractionIconSize;
                    }
                    else
                    {
                        UpdateInteractionUI(interactableObject);
                    }

                    _highlightInteractableObjectController = interactableObject.highlightInteractableObjectController;
                    _highlightInteractableObjectController?.ChangeColor(Color.red);
                }
            }
            else
            {
                _playerArms.SetPickingUp(false);
                _playerInventory.ShowCurrentIndexItem();
            }
        }
        else
        {
            _playerArms.SetPickingUp(false);
            _playerInventory.ShowCurrentIndexItem();
            ResetHighlight();
            _examineObjectController.objectToExamine = null;
        }

        if (interactableObject != _previousInteractable)
        {
            _previousInteractable = interactableObject;
            if (interactableObject != null && !_inPuzzle)
            {
                ResetInteraction();
            }
        }
        
        if ((Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKeyOne)
             || Input.GetKeyDown(InGameSettingsManager.Instance.objectInteractionKeyTwo))
            && _examineObjectController != null
            && !_examineObjectController.isExaminingObject)
        {
            if (interactableObject != null && _allowInteraction && _previousInteractable != null)
            {
                _playerArms.SetPickedUp();
                interactionImage.sprite = defaultInteractionIcon;
                interactionImage.rectTransform.sizeDelta = defaultIconSize;
                interactableObject.onInteraction?.Invoke();
                StartCoroutine(InteractionSpamPrevention());
                _examineObjectController.objectToExamine = null;
            }
        }
        else if (Input.GetKeyDown(InGameSettingsManager.Instance.itemExaminationInteractionKey)
                 && interactableObject != null
                 && interactableObject.TryGetComponent<ExaminableObject>(out var examinableObject)
                 && !examinableObject.isExamining)
        {
            interactionImage.sprite = defaultInteractionIcon;
            interactionImage.rectTransform.sizeDelta = defaultIconSize;
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

        if (interactableObject != null)
        {
            UpdateInteractionUI(null);
            interactableObject = null;
        }
    }

    public bool IsLookingAtInteractableObject(GameObject target)
    {
        return interactableObject != null && interactableObject.gameObject == target;
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

    public IEnumerator InteractionSpamPrevention()
    {
        _allowInteraction = false;
        UpdateInteractionUI(interactableObject);
        yield return new WaitForSeconds(spamPreventionTime);
        _allowInteraction = true;
        UpdateInteractionUI(interactableObject);
    }

    private void ResetInteraction()
    {
        interactableObject = null;
        UpdateInteractionUI(null);
        _highlightInteractableObjectController = null;

        _playerArms.SetPickingUp(false);
        _playerInventory.ShowCurrentIndexItem();
        DialogueController dialogueController = FindObjectOfType<DialogueController>();

        if (dialogueController != null && !_pauseManager.GameIsPaused)
        {
            dialogueController.ResetDialogue();
            dialogueController.DisableDialogueBox();
        
            //GlobalCursorManager.Instance.DisableCursor();
        }
    }
}
