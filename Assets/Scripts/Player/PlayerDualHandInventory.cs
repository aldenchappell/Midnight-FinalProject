using UnityEngine;

public class PlayerDualHandInventory : MonoBehaviour
{
    [Header("Item Hand Location")]
    [SerializeField] Transform handPosition;

    private GameObject[] _inventorySlots;
    public int currentIndexSelected;

    public GameObject[] GetInventory
    {
        get
        {
            return _inventorySlots;
        }
    }

    public bool PickedUp { get; private set; } // New property to track if any item is picked up

    private void Start()
    {
        _inventorySlots = new GameObject[2];
        currentIndexSelected = 0;
        PickedUp = false; // Initialize picked up state
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
    // Swap current interactable object in hand for another in the environment
    private void SwapObjectsInInventory(GameObject newObject)
    {
        if (_inventorySlots[currentIndexSelected] != null)
        {
            _inventorySlots[currentIndexSelected].SetActive(true);
            _inventorySlots[currentIndexSelected].transform.position = newObject.transform.position;
            _inventorySlots[currentIndexSelected].transform.parent = null;
        }
        newObject.transform.parent = this.gameObject.transform;
        _inventorySlots[currentIndexSelected] = newObject;
        ShowCurrentIndexItem();
        PickedUp = true; // Set picked up state to true when an item is picked up
    }

    // Place interactable object in hand in the shadow position
    private void PlaceObjectFromInventory(GameObject objectPosition)
    {
        if (_inventorySlots[currentIndexSelected] != null)
        {
            if(_inventorySlots[currentIndexSelected].transform.tag == objectPosition.transform.tag)
            {
                if(objectPosition.transform.parent != null)
                {
                    _inventorySlots[currentIndexSelected].transform.parent = objectPosition.transform.parent;
                }
                else
                {
                    _inventorySlots[currentIndexSelected].transform.parent = null; 
                }
                
                _inventorySlots[currentIndexSelected].transform.position = objectPosition.transform.position;
                _inventorySlots[currentIndexSelected].transform.eulerAngles = objectPosition.transform.eulerAngles;
                _inventorySlots[currentIndexSelected].GetComponent<InteractableObject>().onPlaceObject.Invoke();
                Destroy(_inventorySlots[currentIndexSelected].GetComponent<InteractableObject>());
                Destroy(objectPosition);
                _inventorySlots[currentIndexSelected] = null;
            }
        }
    }

    // Special place functionality for easier use in puzzles
    public void PlaceObjectInPuzzle(GameObject objectPosition, int index, GameObject parentObject)
    {
        if (_inventorySlots[index] != null)
        {
            if (_inventorySlots[index].transform.tag == objectPosition.transform.tag)
            {
                if (parentObject != null)
                {
                    _inventorySlots[index].transform.parent = parentObject.transform;
                }
                else
                {
                    _inventorySlots[index].transform.parent = null;
                }
                _inventorySlots[index].transform.position = objectPosition.transform.position;
                _inventorySlots[index].transform.eulerAngles = objectPosition.transform.eulerAngles;
                _inventorySlots[index].transform.localScale = objectPosition.transform.localScale;
                _inventorySlots[index].GetComponent<InteractableObject>().onPlaceObject.Invoke();
                Destroy(_inventorySlots[index].GetComponent<InteractableObject>());
                Destroy(_inventorySlots[index].GetComponent<Collider>());
                Destroy(objectPosition);
                _inventorySlots[index].SetActive(true);
                _inventorySlots[index] = null;
            }
        }
    }

    private void RemoveObjectInInventory(GameObject obj)
    {
        foreach(GameObject item in _inventorySlots)
        {
            print(obj.name);
            if(item.name.Contains(obj.name))
            {
                int index = System.Array.IndexOf(_inventorySlots, item);
                Destroy(_inventorySlots[index]);
                _inventorySlots[index] = null;
                break;
            }
        }
    }
    #endregion
    private void ShowCurrentIndexItem()
    {
        foreach(GameObject obj in _inventorySlots)
        {
            if(obj != null)
            {
                obj.SetActive(false);
            }
        }
        if (_inventorySlots[currentIndexSelected] != null)
        {
            _inventorySlots[currentIndexSelected].SetActive(true);
            _inventorySlots[currentIndexSelected].transform.parent = this.gameObject.transform;
            _inventorySlots[currentIndexSelected].transform.localPosition = handPosition.localPosition;
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
}
