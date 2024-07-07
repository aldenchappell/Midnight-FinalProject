using System.Collections.Generic;
using UnityEngine;

public class KeyCubbyController : MonoBehaviour
{
    public List<GameObject> keySlots = new List<GameObject>();
    public List<InteractableObject> interactableKeyColliders = new List<InteractableObject>();

    private PlayerKeyController _playerKeyController;
    private AudioSource _audio;
    [SerializeField] private AudioClip placeKeySound;
    [SerializeField] private AudioClip invalidKeyPlacementSound;

    private bool _isReturnKeyObjectiveCompleted;
    private Objective _returnKeyObjective;
    private ObjectiveController _objectiveController;

    private SaveData _saveData;

    private void Awake()
    {
        _playerKeyController = FindObjectOfType<PlayerKeyController>();
        _audio = GetComponent<AudioSource>();
        _objectiveController = FindObjectOfType<ObjectiveController>();
    }

    private void Start()
    {
        _saveData = SaveSystem.Load();
        InitializeKeySlots();
        SetupInteractionEvents();
        
        if (LevelCompletionManager.Instance.hasCompletedLobby)
        {
            SetupLobbyObjectives();
        }
        
        // if(InGameSettingsManager.Instance.isFirstLaunch)
        // {
        //     ResetCubby();
        //     Debug.Log("Resetting cubby due to first launch");
        // }
    }

    private void SetupLobbyObjectives()
    {
        if (LevelCompletionManager.Instance.hasKey && !_isReturnKeyObjectiveCompleted)
        {
            bool returnKeyObjectiveExists = _objectiveController.ObjectiveExists("* Return the key to the front desk.");

            if (!returnKeyObjectiveExists)
            {
                _returnKeyObjective = gameObject.AddComponent<Objective>();
                _returnKeyObjective.description = "* Return the key to the front desk.";
                _returnKeyObjective.order = 2;

                _objectiveController.RegisterObjective(_returnKeyObjective);
            }
        }
    }

    private void InitializeKeySlots()
    {
        for (int i = 0; i < keySlots.Count; i++)
        {
            bool keyPlaced = IsKeyPlaced(i);
            keySlots[i].SetActive(keyPlaced);
            Debug.Log($"Key slot {i} initialized to {keyPlaced}");
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
            keySlots[keyIndex].SetActive(true);
            _audio.PlayOneShot(placeKeySound);

            if (!_saveData.placedKeys.Contains(keyIndex))
            {
                _saveData.placedKeys.Add(keyIndex);
            }

            SaveSystem.Save(_saveData);

            LevelCompletionManager.Instance.hasKey = false;
            Debug.Log($"Key placed in slot {keyIndex}");
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
        return _saveData != null && _saveData.placedKeys.Contains(keyIndex);
    }

    public void ResetCubby()
    {
        _saveData.placedKeys.Clear();
        SaveSystem.Save(_saveData);
        foreach (var slot in keySlots)
        {
            slot.SetActive(false);
        }
        Debug.Log("Cubby reset");
    }
}