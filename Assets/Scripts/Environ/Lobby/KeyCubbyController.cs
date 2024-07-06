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

    private void Awake()
    {
        _playerKeyController = FindObjectOfType<PlayerKeyController>();
        _audio = GetComponent<AudioSource>();
        _objectiveController = FindObjectOfType<ObjectiveController>();
        if (LevelCompletionManager.Instance.hasCompletedLobby)
        {
            SetupLobbyObjectives();
        }
    }

    private void Start()
    {
        InitializeKeySlots();
        SetupInteractionEvents();
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
            PlayerPrefs.SetInt($"KeyPlaced_{keyIndex}", 1);

            LevelCompletionManager.Instance.hasKey = false;
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
            return PlayerPrefs.GetInt($"KeyPlaced_{keyIndex}", 0) == 1;
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
    }
}
