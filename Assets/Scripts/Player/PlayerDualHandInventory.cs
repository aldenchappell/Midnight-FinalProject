using UnityEngine;

public class PlayerDualHandInventory : MonoBehaviour
{
    [Header("Item Hand Location")]
    public Transform[] handPositions;
    [SerializeField] Transform defaultHand;
    [SerializeField] Transform skullOfHandPosition;

    public GameObject[] _inventorySlots;
    public int currentIndexSelected;

    private PlayerArmsAnimationController _armsAnimationController;
    public GameObject[] GetInventory
    {
        get
        {
            return _inventorySlots;
        }
    }
    public GameObject GetCurrentHandItem
    {
        get
        {
            return _inventorySlots[currentIndexSelected];
        }
    }
 
    public bool PickedUp { get; private set; } // New property to track if any item is picked up

    private void Start()
    {
        _inventorySlots = new GameObject[2];
        currentIndexSelected = 0;
        PickedUp = false; // Initialize picked up state
        
        _armsAnimationController = FindObjectOfType<PlayerArmsAnimationController>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentIndexSelected = 0;
            ShowCurrentIndexItem();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentIndexSelected = 1;
            ShowCurrentIndexItem();
        }
    }

    #region Public methods
    // Adjust inventory when picking up a new interactable object
    public GameObject AdjustInventorySlots
    {
        set
        {
            SwapObjectsInInventory(value);
        }
    }

    // Place interactable object in inventory onto shadow position and adjust inventory
    public GameObject PlaceObject
    {
        set
        {
            PlaceObjectFromInventory(value);
        }
    }

    public GameObject RemoveObject
    {
        set
        {
            RemoveObjectInInventory(value);
        }
    }
    #endregion
    
    #region Inventory Methods
    private void SwapObjectsInInventory(GameObject newObject)
    {
        if (_inventorySlots[currentIndexSelected] != null)
        {
            _inventorySlots[currentIndexSelected].transform.parent = null;
            _inventorySlots[currentIndexSelected].SetActive(true);
            _inventorySlots[currentIndexSelected].layer = LayerMask.NameToLayer("InteractableObject");
            
            if(!_inventorySlots[currentIndexSelected].CompareTag("Skull"))
                _inventorySlots[currentIndexSelected].GetComponent<Collider>().enabled = true;
    
            // Functionality to place skull in proper position;
            if (_inventorySlots[currentIndexSelected].GetComponent<SkullDialogue>())
            {
                if(newObject.transform.Find("GhostPlacement"))
                {
                    Debug.Log("Moving to " + newObject.name + "'s ghost placement position.");
                    _inventorySlots[currentIndexSelected].transform.position =
                        newObject.transform.Find("GhostPlacement").position;
                    _inventorySlots[currentIndexSelected].transform.eulerAngles =
                        newObject.transform.Find("GhostPlacement").eulerAngles;
                    _inventorySlots[currentIndexSelected].transform.GetComponent<MeshRenderer>().enabled = true;
                    _inventorySlots[currentIndexSelected].transform.GetComponent<Collider>().enabled = true;
                }
                else
                {
                    _inventorySlots[currentIndexSelected].transform.position = newObject.transform.position;
                    _inventorySlots[currentIndexSelected].transform.eulerAngles = newObject.transform.eulerAngles;
                }
                
            }
            else
            {
                _inventorySlots[currentIndexSelected].transform.position = newObject.transform.position;
                _inventorySlots[currentIndexSelected].transform.eulerAngles = newObject.transform.eulerAngles;
            }
        }

        newObject.transform.parent = this.gameObject.transform;
        newObject.layer = LayerMask.NameToLayer("Default");
        _inventorySlots[currentIndexSelected] = newObject;
        ShowCurrentIndexItem();
        PickedUp = true;
    }

    private void PlaceObjectFromInventory(GameObject obj)
    {
        if (_inventorySlots[currentIndexSelected] != null)
        {
            if(_inventorySlots[currentIndexSelected].transform.tag == obj.transform.tag)
            {
                if(obj.transform.parent != null)
                {
                    _inventorySlots[currentIndexSelected].transform.parent = obj.transform.parent;
                }
                else
                {
                    _inventorySlots[currentIndexSelected].transform.parent = null; 
                    
                }
                
                _inventorySlots[currentIndexSelected].transform.position = obj.transform.position;
                _inventorySlots[currentIndexSelected].transform.eulerAngles = obj.transform.eulerAngles;
                _inventorySlots[currentIndexSelected].GetComponent<Collider>().enabled = true;
                obj.GetComponent<InteractableObject>().onPlaceObject.Invoke();
                Destroy(_inventorySlots[currentIndexSelected].GetComponent<InteractableObject>());
                Destroy(obj);
                _inventorySlots[currentIndexSelected] = null;
                
                if (_armsAnimationController != null)
                {
                    //_armsAnimationController.SetHoldingObject(false); // Call SetHoldingObject here
                }
            }
        }
    }

    public void PlaceObjectInPuzzle(GameObject obj)
    {
        foreach(GameObject item in _inventorySlots)
        {
            if(item != null)
            {
                if (item.CompareTag(obj.tag))
                {
                    int index = System.Array.IndexOf(_inventorySlots, item);
                    if (obj.transform.parent != null)
                    {
                        _inventorySlots[index].transform.parent = obj.transform.parent;
                    }
                    else
                    {
                        _inventorySlots[index].transform.parent = null;
                    }

                    _inventorySlots[index].transform.position = obj.transform.position;
                    _inventorySlots[index].transform.eulerAngles = obj.transform.eulerAngles;
                    _inventorySlots[index].SetActive(true);

                    obj.GetComponent<InteractableObject>().onPlaceObject.Invoke();
                    Destroy(_inventorySlots[index].GetComponent<InteractableObject>());
                    Destroy(obj);
                    _inventorySlots[index] = null;
                }
            }
           
        }
        
    }

    private void RemoveObjectInInventory(GameObject obj)
    {
        foreach(GameObject item in _inventorySlots)
        {
            if(item != null)
            {
                if (item.CompareTag(obj.tag))
                {
                    int index = System.Array.IndexOf(_inventorySlots, item);
                    Destroy(_inventorySlots[index]);
                    _inventorySlots[index] = null;
                    break;
                }
            } 
        }
    }
    #endregion
    
    public void ShowCurrentIndexItem()
    {
        HideHandItem(); // Hide any currently held item

        if (_inventorySlots[currentIndexSelected] != null)
        {
            GameObject item = _inventorySlots[currentIndexSelected];

            // Set item position and visibility
            item.SetActive(true);
            
            if (item.GetComponent<ObjectSize>() && handPositions.Length > 0)
            {
                item.transform.parent = handPositions[item.GetComponent<ObjectSize>().objectSize.handPositionIndex].parent.gameObject.transform;
                item.transform.localPosition = handPositions[item.GetComponent<ObjectSize>().objectSize.handPositionIndex].localPosition;
                item.transform.localEulerAngles = handPositions[item.GetComponent<ObjectSize>().objectSize.handPositionIndex].localEulerAngles;
                //Debug.Log(
                    //item.gameObject.name + " was moved to the " +
                   // item.GetComponent<ObjectSize>().objectSize.handPositionIndex + " index.");
            }
            else
            {
                //if the object hasn't been assigned an ObjectSize, default to small object size.
                item.transform.parent = defaultHand.parent.gameObject.transform;
                item.transform.localPosition = defaultHand.localPosition;
                item.transform.localEulerAngles = defaultHand.localEulerAngles;
                //Debug.Log("There is no ObjectSize component on the "
                         // + item.gameObject.name + ". Defaulting to small.");
            }
            
            item.GetComponent<Collider>().enabled = false;

            // Special handling for Skull item
            if (item.CompareTag("Skull"))
            {
                item.GetComponent<MeshRenderer>().enabled = true;
                GameObject.Find("SkullDialogueHolder").GetComponent<AudioSource>().volume = 1f;
            }

            // Update arms animation controller with the object held
            if (_armsAnimationController != null)
            {
                //_armsAnimationController.HoldObject(item);
                //_armsAnimationController.SetHoldingObject(true);
            }
        }
    }
    
    public bool IsSkullInFirstSlot()
    {
        if (_inventorySlots.Length > 0 && _inventorySlots[1] != null)
        {
            return _inventorySlots[1].GetComponent<SkullDialogue>() != null;
        }
        return false;
    }

    public bool MatchPuzzlePieceInInventory(PuzzlePiece activePuzzlePiece)
    {
        if (_inventorySlots.Length > 0 && _inventorySlots[1] != null)
        {
            return _inventorySlots[1].GetComponent<PuzzlePiece>().puzzlePieceName
                   == activePuzzlePiece.GetComponent<PuzzlePiece>().puzzlePieceName;
        }

        return false;
    }

    public void HideHandItem()
    {
        foreach (GameObject item in _inventorySlots)
        {
            if (item != null)
            {
                if (item.CompareTag("Skull"))
                {
                    item.GetComponent<Collider>().enabled = false;
                    item.transform.position = skullOfHandPosition.position;
                    item.GetComponent<MeshRenderer>().enabled = false;
                    GameObject.Find("SkullDialogueHolder").GetComponent<AudioSource>().volume = .5f;
                }
                else
                {
                    item.SetActive(false);
                }
            }
        }
    }
    
    private int GetObjectIndex(string tag)
    {
        return 0;
    }

    private float CalculateObjectSize(GameObject obj)
    {
        if(obj)
            return obj.transform.localScale.magnitude;

        return 1.0f; 
    }
}