using System.Collections.Generic;
using UnityEngine;

public class KeyCubbyController : MonoBehaviour
{
    public List<GameObject> keySlots = new List<GameObject>();
    public List<InteractableObject> interactableKeyColliders = new List<InteractableObject>();
    public List<CubbyKey> cubbyKeys = new List<CubbyKey>();
    
    private PlayerKeyController _playerKeyController;
    private AudioSource _audio;
    [SerializeField] private AudioClip placeKeySound;
    [SerializeField] private AudioClip invalidKeyPlacementSound;

    private bool _isReturnKeyObjectiveCompleted;
    private Objective _returnKeyObjective;
    private ObjectiveController _objectiveController;

    private void Awake()
    {
        _playerKeyController = FindObjectOfType<PlayerKeyController>();
        _audio = GetComponent<AudioSource>();
        _objectiveController = FindObjectOfType<ObjectiveController>();
    }

    private void Start()
    {
        InitializeKeySlots();
        SetupInteractionEvents();
        ActivateReturnedKeys();


        if (LevelCompletionManager.Instance.hasCompletedLobby)
        {
            SetupLobbyObjectives();
        }

        //Might cause issues resetting progress before player is finished?

        if (SceneTransitionManager.PreviouslyLoadedSceneName == "MAINMENU")
        {
            Debug.Log(SceneTransitionManager.PreviouslyLoadedSceneName);
            ResetCubby();
        }

    }
    
    private void SetupLobbyObjectives()
    {
        if (LevelCompletionManager.Instance.hasKey && !_isReturnKeyObjectiveCompleted)
        {
            bool returnKeyObjectiveExists = _objectiveController.ObjectiveExists("* Return the key to the front desk.");

            if (!returnKeyObjectiveExists)
            {
                // Objective to return key to front desk
                _returnKeyObjective = gameObject.AddComponent<Objective>();
                _returnKeyObjective.description = "* Return the key to the front desk.";
                _returnKeyObjective.order = 3;

                _objectiveController.RegisterObjective(_returnKeyObjective);
            }
        }
    }

    private void InitializeKeySlots()
    {
        for (int i = 0; i < keySlots.Count; i++)
        {
            //bool keyPlaced = IsKeyPlaced(i);
            keySlots[i].SetActive(cubbyKeys[i].cubbyKey.placed);
            Debug.Log($"Key at index {keySlots[i]} active state is {cubbyKeys[i].cubbyKey.placed}");
        }
    }

    private void SetupInteractionEvents()
    {
        for (int i = 0; i < interactableKeyColliders.Count; i++)
        {
            int keyIndex = i;
            interactableKeyColliders[i].onInteraction.AddListener(() => TryPlaceKeyInCubby(keyIndex));
        }
    }

    private void TryPlaceKeyInCubby(int keyIndex)
    {
        if (_playerKeyController != null && _playerKeyController.keys > 0)
        {
            if (IsSlotAvailable(keyIndex))
            {
                _playerKeyController.PlaceKeyInCubby(keyIndex);
                cubbyKeys[keyIndex].cubbyKey.placed = true;
                Debug.Log($"Key at index {cubbyKeys[keyIndex]} has been placed.");
                if (_returnKeyObjective != null)
                {
                    _returnKeyObjective.CompleteObjective();
                    _isReturnKeyObjectiveCompleted = true;
                }
            }
            else
            {
                _audio.PlayOneShot(invalidKeyPlacementSound);
                Debug.Log("Cubby slot is not available.");
            }
        }
        else
        {
            _audio.PlayOneShot(invalidKeyPlacementSound);
            Debug.Log("Player does not have a key to place.");
        }
    }

    public void PlaceKey(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keySlots.Count)
        {
            cubbyKeys[keyIndex].keyObject.SetActive(true);
            cubbyKeys[keyIndex].cubbyKey.placed = true;
            cubbyKeys[keyIndex].particles.SetActive(false);
            _audio.PlayOneShot(placeKeySound);
            

            LevelCompletionManager.Instance.hasKey = false;
            //Added this line for fix: Owen
            LevelCompletionManager.Instance._keysReturned++;
        }
        else
        {
            _audio.PlayOneShot(invalidKeyPlacementSound);
            Debug.Log("Invalid key index for placing key in cubby.");
        }
    }

    public bool IsSlotAvailable(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keySlots.Count)
        {
            return !keySlots[keyIndex].activeSelf;
        }

        _audio.PlayOneShot(invalidKeyPlacementSound);
        Debug.LogWarning("Invalid key index for checking slot availability in cubby.");
        return false;
    }


    public bool IsKeyPlaced(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keySlots.Count)
        {
            return cubbyKeys[keyIndex].cubbyKey.placed;
        }
        
        _audio.PlayOneShot(invalidKeyPlacementSound);
        Debug.LogWarning("Invalid key index for checking placed key in cubby.");
        return false;
    }

    public void ResetCubby()
    {
        foreach (var slot in keySlots)
        {
            slot.SetActive(false);
        }

        foreach (var key in cubbyKeys)
        {
            key.cubbyKey.placed = false;
        }
    }

    //Added this method as part of fix: Owen
    private void ActivateReturnedKeys()
    {
        if(LevelCompletionManager.Instance._keysReturned > 0)
        {
            for(int i = 0; i < LevelCompletionManager.Instance._keysReturned; i++)
            {
                cubbyKeys[i].cubbyKey.placed = true;
                cubbyKeys[i].keyObject.SetActive(true);
            }
        }
    }
}
