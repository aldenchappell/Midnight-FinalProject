using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class BabyBlockPuzzle : MonoBehaviour
{
    [Header("Puzzle UI")]
    [SerializeField] GameObject puzzleUI;
    [SerializeField] GameObject puzzleHud;
    [Header("Animation Child")]
    [SerializeField] GameObject animationChild;
    [Header("Complete Material")]
    [SerializeField] Material completeMaterial;
    [Header("AudioClips")]
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip failSound;

    private PlayerDualHandInventory  _playerInv;
    private FirstPersonController _FPC;
    private CinemachineVirtualCamera _playerCam;
    private CinemachineVirtualCamera _puzzleCam;
    private Puzzle _puzzle;
    private Camera _mainCam;
    private AudioSource _audioSource;
    private Animator _animator;
    private PatrolSystemManager _patrol;
    private GlobalCursorManager _cursor;
    private Camera _mainCamera;
    private PuzzleEscape _puzzleEscape;
    private GameObject _arms;

    private bool _isActive;
    private bool _canExit;
    private bool _canAnimate;
    private int _correctObjectsPlaced;
    private Vector3 _originalAnimationChildRotation;

    private void Awake()
    {
        _arms = GameObject.Find("Arms");
        _playerCam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _puzzleCam = GameObject.Find("BabyBlockPuzzleCam").GetComponent<CinemachineVirtualCamera>();
        _mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        _playerInv = GameObject.FindFirstObjectByType<PlayerDualHandInventory>();
        _mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();

        _FPC = GameObject.FindFirstObjectByType<FirstPersonController>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();

        _canAnimate = true;
        _canExit = false;

        _patrol = GameObject.Find("DemonPatrolManager").GetComponent<PatrolSystemManager>();
        _puzzleEscape = GetComponent<PuzzleEscape>();
        GlobalCursorManager.Instance = _cursor;
    }

    private void Update()
    {
        if(_isActive)
        {
            CheckForInput();
        }
    }

    #region Activation and Input
    //Turn on or off puzzle interaction
    public void ActivatePuzzle()
    {
        puzzleUI.SetActive(!puzzleUI.activeSelf);
        _puzzleEscape.ChangeIsActive();
        if (puzzleUI.activeSelf)
        {
            puzzleHud.SetActive(true);
            _arms.SetActive(false);
            _mainCamera.transform.GetChild(2).gameObject.SetActive(false);
            GetComponent<Collider>().enabled = false;
            _FPC.ToggleCanMove();
            _playerCam.Priority = 0;
            _puzzleCam.Priority = 10;

            List<Transform> children = new List<Transform>();
            children.AddRange(animationChild.GetComponentsInChildren<Transform>());
            children.Remove(animationChild.transform);

            foreach (Transform child in children)
            {
                if (child.GetComponent<Collider>() != null)
                {
                    child.GetComponent<Collider>().enabled = true;
                }
            }

            _isActive = true;
            FindObjectOfType<GlobalCursorManager>().EnableCursor();
            Invoke("SetCanExit", 1f);
        }
        else
        {
            puzzleHud.SetActive(false);
            _arms.SetActive(false);
            _mainCamera.transform.GetChild(2).gameObject.SetActive(true);
            GetComponent<Collider>().enabled = true;
            FindObjectOfType<GlobalCursorManager>().DisableCursor();
            _FPC.ToggleCanMove();
            _playerCam.Priority = 10;
            _puzzleCam.Priority = 0;

            List<Transform> children = new List<Transform>();
            children.AddRange(animationChild.GetComponentsInChildren<Transform>());
            children.Remove(animationChild.transform);

            foreach (Transform child in children)
            {
                if(child.GetComponent<Collider>() != null)
                {
                    child.GetComponent<Collider>().enabled = false;
                }
            }

            _isActive = false;
            _canExit = false;
        }
    }
    //Check for inputs once puzzle is active.
    private void CheckForInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateObject(Vector3.up, -90);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            RotateObject(Vector3.up, 90);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            RotateObject(Vector3.forward, 90);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            RotateObject(Vector3.forward, -90);
        }
        if (Input.GetMouseButtonDown(0) && _canAnimate)
        {
            RaycastToMousePosition();
        }
        if(Input.GetMouseButtonDown(1) && _canExit)
        {
            ActivatePuzzle();
        }
    }

    private void SetCanExit()
    {
        _canExit = !_canExit;
    }

    #endregion

    #region Rotation
    //Rotate baby block cube 
    private void RotateObject(Vector3 axis, float angle)
    {
        Vector3 newRotation = animationChild.transform.rotation.eulerAngles + axis * angle;
        animationChild.transform.rotation = Quaternion.Lerp(animationChild.transform.rotation, Quaternion.Euler(newRotation), Time.deltaTime * 2);
    }

    #endregion

    #region Mouse Raycast and Check Objects
    //Check for mouse clicks
    private void RaycastToMousePosition()
    {
        RaycastHit hit;
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if(!objectHit.CompareTag("Untagged"))
            {
                CheckForCorrectObject(objectHit);
            }
        }
    }
    //Check for correct object in inventory to be placed.
    private void CheckForCorrectObject(Transform slot)
    {
        GameObject rightItem = null;
        GameObject[] currentItems = _playerInv.GetInventory;
        foreach(GameObject item in currentItems)
        {
            if(item != null)
            {
                if (item.transform.CompareTag(slot.tag))
                {
                    rightItem = item;
                }
            }
        }
        if(rightItem == null)
        {
            PlayAudioClip(failSound);
            _patrol.DecreaseTimeToSpawn = 10;
        }
        else
        {
            PlayAudioClip(winSound);
            StartCoroutine(PlayAnimation(slot.gameObject, System.Array.IndexOf(currentItems, rightItem), animationChild, rightItem));
            _correctObjectsPlaced++;
        }
    }

    private void CheckForCompletion()
    {
        if(_correctObjectsPlaced >= 4)
        {
            List<Transform> children = new List<Transform>();
            children.AddRange(animationChild.GetComponentsInChildren<Transform>());
            children.Remove(animationChild.transform);
            for(int i = 0; i < animationChild.transform.childCount; i++)
            {
                if (animationChild.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                {
                    animationChild.transform.GetChild(i).GetComponent<MeshRenderer>().material = completeMaterial;
                }
            }
            GameObject.FindObjectOfType<DaVinciPuzzle>()._hasBlockBeenSolved = true;

            if(GetComponent<Puzzle>() != null)
            {
                GetComponent<Puzzle>().CompletePuzzle();
            }
            else
            {
                Debug.Log("Puzzle Completed");
            }
            
        }
    }
    #endregion

    private IEnumerator PlayAnimation(GameObject slot, int index, GameObject parent, GameObject heldObject)
    {
        _canAnimate = false;
        switch (slot.name)
        {
            case "BBPiece1":
                slot.GetComponent<MeshRenderer>().material = _playerInv.GetInventory[index].GetComponent<MeshRenderer>().material;
                _animator.SetTrigger("Block1");
                break;
            case "BBPiece2":
                slot.GetComponent<MeshRenderer>().material = _playerInv.GetInventory[index].GetComponent<MeshRenderer>().material;
                _animator.SetTrigger("Block2");
                break;
            case "BBPiece3":
                slot.GetComponent<MeshRenderer>().material = _playerInv.GetInventory[index].GetComponent<MeshRenderer>().material;
                _animator.SetTrigger("Block3");
                break;
            case "BBPiece4":
                slot.GetComponent<MeshRenderer>().material = _playerInv.GetInventory[index].GetComponent<MeshRenderer>().material;
                _animator.SetTrigger("Block4");
                break;
        }
        yield return new WaitForSeconds(1f);
        _animator.SetTrigger("Return");
        _playerInv.RemoveObject = heldObject;
        CheckForCompletion();
        _canAnimate = true;
    }

    private void PlayAudioClip(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

}
