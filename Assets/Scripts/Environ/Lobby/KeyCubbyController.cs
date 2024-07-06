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

    private void Awake()
    {
        _playerKeyController = FindObjectOfType<PlayerKeyController>();
        _audio = GetComponent<AudioSource>();

        if (!LevelCompletionManager.Instance.hasCompletedLobby) return;
    }

    private void Start()
    {
        InitializeKeySlots();
        SetupInteractionEvents();
        SetupLobbyObjectives();
    }

    private void SetupLobbyObjectives()
    {
        if (LevelCompletionManager.Instance.hasCompletedLobby)
        {
            List<Objective> objectives = new List<Objective>();

            if (LevelCompletionManager.Instance.hasKey && !_isReturnKeyObjectiveCompleted)
            {
                // objective to return key to front desk
                Objective returnKeyObjective = new Objective();
                returnKeyObjective.description = "* Return the key to the front desk.";
                returnKeyObjective.order = 2;

                // add interaction listener to each interactable key collider
                foreach (var interactable in interactableKeyColliders)
                {
                    interactable.onInteraction.AddListener(() =>
                    {
                        returnKeyObjective.CompleteObjective();
                        _isReturnKeyObjectiveCompleted = true;
                        FindObjectOfType<TaskController>().UpdateObjectiveText(objectives.ToArray());
                    });
                }

                objectives.Add(returnKeyObjective);
            }

            // check if all levels are completed to add exit objective
            if (LevelCompletionManager.Instance.allLevelsCompleted)
            {
                var revolvingDoors = FindObjectOfType<LobbyDoorExit>();
                Objective exitObjective = new Objective();
                exitObjective.description = "* Exit the hotel through the revolving doors.";
                exitObjective.order = 3;

                revolvingDoors.GetComponent<InteractableObject>().onInteraction.AddListener(() =>
                {
                    exitObjective.CompleteObjective();
                    LevelCompletionManager.Instance.CompleteObjective(exitObjective);
                });

                objectives.Add(exitObjective);
            }

            foreach (var objective in objectives)
            {
                FindObjectOfType<ObjectiveController>().RegisterObjective(objective);
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
