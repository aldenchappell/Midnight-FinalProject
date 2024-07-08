using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDualHandInventory : MonoBehaviour
{
    [Header("Item Hand Location")]
    public Transform[] handPositions;
    [SerializeField] Transform defaultHand;
    [SerializeField] Transform skullOfHandPosition;

    
    public GameObject[] _inventorySlots;
    public int currentIndexSelected;

    private PlayerArmsAnimationController _armsAnimationController;
    private Dictionary<GameObject, Vector3> _originalScales = new Dictionary<GameObject, Vector3>();

    [SerializeField] private Image[] inventorySlotImages = new Image[2];
    [SerializeField] private Image[] inventorySlotBorders = new Image[2];
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
        UpdateInventoryUI();
        _armsAnimationController = FindObjectOfType<PlayerArmsAnimationController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentIndexSelected = 0;
            ShowCurrentIndexItem();
            UpdateInventoryUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentIndexSelected = 1;
            ShowCurrentIndexItem();
            UpdateInventoryUI();
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
                    _inventorySlots[currentIndexSelected].transform.localRotation =
                        newObject.transform.Find("GhostPlacement").localRotation;
                    _inventorySlots[currentIndexSelected].transform.GetComponent<MeshRenderer>().enabled = true;
                    _inventorySlots[currentIndexSelected].transform.GetComponent<Collider>().enabled = true;
                }
                else
                {
                    _inventorySlots[currentIndexSelected].transform.position = newObject.transform.position;
                    _inventorySlots[currentIndexSelected].transform.eulerAngles = newObject.transform.eulerAngles;
                    _inventorySlots[currentIndexSelected].transform.GetComponent<MeshRenderer>().enabled = true;
                    _inventorySlots[currentIndexSelected].transform.GetComponent<Collider>().enabled = true;
                }
                
            }
            else
            {
                _inventorySlots[currentIndexSelected].transform.position = newObject.transform.position;
                _inventorySlots[currentIndexSelected].transform.eulerAngles = newObject.transform.eulerAngles;
            }
            UpdateInventoryUI();
        }

        newObject.transform.parent = this.gameObject.transform;

        if (!_originalScales.ContainsKey(newObject))
        {
            _originalScales.Add(newObject, newObject.transform.localScale);
        }
        //newObject.transform.localScale = _originalScales[newObject];
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
                UpdateInventoryUI();
                obj.GetComponent<InteractableObject>().onPlaceObject.Invoke();
                _inventorySlots[currentIndexSelected].GetComponent<InteractableObject>().onPlaceObject.Invoke();
                Destroy(_inventorySlots[currentIndexSelected].GetComponent<InteractableObject>());
                Destroy(obj);
                _inventorySlots[currentIndexSelected] = null;
                
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
                    UpdateInventoryUI();
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
                    UpdateInventoryUI();
                    _inventorySlots[index] = null;
                    break;
                }
            } 
        }
    }
    #endregion
    
    public void ShowCurrentIndexItem()
    {
        HideHandItem();

        if (_inventorySlots[currentIndexSelected] != null)
        {
            GameObject item = _inventorySlots[currentIndexSelected];
            item.SetActive(true);

            
            // if (item.GetComponent<ObjectSize>() && handPositions.Length > 0)
            // {
            //     // Set position and rotation based on ObjectSize
            //     int index = item.GetComponent<ObjectSize>().objectSize.handPositionIndex;
            //     item.transform.parent = handPositions[index].parent;
            //     item.transform.localPosition = handPositions[index].localPosition;
            //     item.transform.localEulerAngles = handPositions[index].localEulerAngles;
            // }
            // else
            {
                // Default to small object size
                item.transform.parent = defaultHand.parent;
                item.transform.localPosition = defaultHand.localPosition;
                item.transform.localEulerAngles = defaultHand.localEulerAngles;
            }
            //
            // if (_originalScales.ContainsKey(item))
            // {
            //     item.transform.localScale = _originalScales[item]; // Reset to the original scale
            // }
            // else
            // {
            //     _originalScales.Add(item, item.transform.localScale);
            // }
            

            item.GetComponent<Collider>().enabled = false;

            if (item.CompareTag("Skull"))
            {
                item.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetChild(0).gameObject.SetActive(true);
                GameObject.Find("SkullDialogueHolder").GetComponent<AudioSource>().volume = 1f;
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
                    item.transform.GetChild(0).gameObject.SetActive(false);
                    GameObject.Find("SkullDialogueHolder").GetComponent<AudioSource>().volume = .5f;
                }
                else
                {
                    item.SetActive(false);
                }
            }
        }
    }

    private void UpdateInventoryUI()
    {
        Color selectedSlotColor = new Color(1, 1, 1, 1f);
        Color inactiveSlotColor = new Color(1, 1, 1, .5f);

        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i] != null)
            {
                if(_inventorySlots[i].GetComponent<InventoryObject>())
                    inventorySlotImages[i].sprite = _inventorySlots[i].GetComponent<InventoryObject>().GetItemImage();
                
                inventorySlotImages[i].color = new Color(
                    inventorySlotImages[i].color.r,
                    inventorySlotImages[i].color.g,
                    inventorySlotImages[i].color.b, 1);//255 alpha
            }
            else
            {
                inventorySlotImages[i].color = new Color(
                    inventorySlotImages[i].color.r,
                    inventorySlotImages[i].color.g,
                    inventorySlotImages[i].color.b, 0);//0 alpha
            }
        }

        switch (currentIndexSelected)
        {
            case 0:
                inventorySlotBorders[0].color = selectedSlotColor;
                inventorySlotBorders[1].color = inactiveSlotColor;
                break;
            case 1:
                inventorySlotBorders[1].color = selectedSlotColor;
                inventorySlotBorders[0].color = inactiveSlotColor;
                break;
        }
    }
}