using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class FuseBox : MonoBehaviour
{
    [SerializeField] private GameObject fuseObject;
    private AudioSource _radioAudio;
    private List<Light> _lobbyLights;
    private List<Material> _lobbyEmissives;
    private ElevatorController _elevator;

    private Animator _animator;

    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _puzzleCam;
    private Camera _mainCam;

    private Collider _boxCollider;

    private PlayerDualHandInventory _inventory;
    private FirstPersonController _FPC;

    private bool _isActive;
    private bool _fuseIn;
    private bool _canExit;

    private Puzzle _puzzle;

    private void Awake()
    {
        _inventory = GameObject.FindAnyObjectByType<PlayerDualHandInventory>();
        _FPC = GameObject.FindFirstObjectByType<FirstPersonController>();
        _mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
        _animator = transform.GetComponent<Animator>();
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();

        _boxCollider = transform.GetComponent<Collider>();
        _isActive = false;
        _puzzle = GetComponent<Puzzle>();
        _lobbyLights = new List<Light>(FindObjectsOfType<Light>());
        _lobbyEmissives = new List<Material>();
        

        _elevator = FindObjectOfType<ElevatorController>();
        _elevator.enabled = false;
        

        _radioAudio = GameObject.Find("LargeRadio").GetComponent<AudioSource>();
        _radioAudio.enabled = false;
    }

    private void Start()
    {
        InitializeLighting();

        if (LevelCompletionManager.Instance.hasCompletedLobby)
        {
            PowerLobby();
            GameObject.Find("Fuse").SetActive(false);
            Destroy(this.gameObject.GetComponent<InteractableObject>());
        }
        else
        {
            ToggleEmissives(false);
        }
    }


    private void Update()
    {
        if (_isActive)
        {
            CheckForInput();
        }
    }

    private void InitializeLighting()
    {
        //lights
        for (int i = 0; i < _lobbyLights.Count; i++)
        {
            if (_lobbyLights[i].gameObject.name == "ExamineObjectLight" || _lobbyLights[i].gameObject.CompareTag("LobbyLight"))
            {
                //print("Removed");
                //Debug.Log(_lobbyLights[i]);
                _lobbyLights.Remove(_lobbyLights[i]);
                i -= 1;
            }
        }

        foreach (Light light in _lobbyLights)
        {
            light.enabled = false;
            
            //add the renderers of the light parents to the emissives list
            Renderer rend = light.GetComponentInParent<Renderer>();
            if (rend != null)
            {
                Material material = rend.material;
                if (material != null && material.IsKeywordEnabled("_EMISSION"))
                {
                    _lobbyEmissives.Add(material);
                }
            }
        }
    }

    private void ToggleEmissives(bool enable)
    {
        //emissives
        foreach (var mat in _lobbyEmissives)
        {
            if (enable)
            {
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    public void ActivatePuzzle()
    {
        _isActive = !_isActive;
        _boxCollider.enabled = !_boxCollider.enabled;
        if (_isActive)
        {
            _playerCam.Priority = 0;
            _puzzleCam.Priority = 10;
            _FPC.ToggleCanMove();
            AnimationsTrigger("Open");
            FindObjectOfType<GlobalCursorManager>().EnableCursor();
            Invoke("ChangeExitBool", 1f);
        }
        else
        {
            _playerCam.Priority = 10;
            _puzzleCam.Priority = 0;
            _FPC.ToggleCanMove();
            FindObjectOfType<GlobalCursorManager>().DisableCursor();
        }
    }

    private void CheckForInput()
    {
        if (Input.GetMouseButtonDown(1) && _canExit)
        {
            ActivatePuzzle();
            _canExit = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            RaycastToMousePosition();
        }
    }

    private void ChangeExitBool()
    {
        _canExit = true;
    }

    private void RaycastToMousePosition()
    {
        RaycastHit hit;
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Fuse"))
            {
                PlaceFuse(objectHit.gameObject);
            }
            if (objectHit.CompareTag("Lever") && _fuseIn)
            {
                LevelCompletionManager.Instance.hasCompletedLobby = true;
                PowerLobby(); // Only trigger power if fuse is in and lever is pulled
                
                PlayerPrefs.SetInt("LobbyPowered", 1);
                PlayerPrefs.Save();
            }
        }
    }


    private void PowerLobby()
    {
        AnimationsTrigger("PowerOn");

        // Enable lobby lights
        foreach (Light light in _lobbyLights)
        {
            light.enabled = true;
        }

        //turn on all lamps and enable emissives
        FindObjectOfType<LampController>().PowerOnLamps();

        _radioAudio.enabled = true;
        _elevator.enabled = true;
    }


    private void PlaceFuse(GameObject fuseShadow)
    {
        GameObject[] currentItems = _inventory.GetInventory;
        foreach (GameObject item in currentItems)
        {
            if (item != null)
            {
                if (item.transform.CompareTag(fuseShadow.tag))
                {
                    AnimationsTrigger("Place");
                    _inventory.RemoveObject = item;
                    _fuseIn = true;
                }
            }
        }

    }

    private void AnimationsTrigger(string trigger)
    {
        _animator.SetTrigger(trigger);

        if (trigger == "PowerOn")
        {
            if (_puzzle == null)
            {
                Debug.LogError("Puzzle component not found");
            }
            else
            {
                _puzzle.CompletePuzzle();
            }

        }
    }

}