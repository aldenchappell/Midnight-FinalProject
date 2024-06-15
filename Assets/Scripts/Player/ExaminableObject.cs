using System;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class ExaminableObject : MonoBehaviour
{
    [HideInInspector] public CinemachineVirtualCamera playerCamera;
    [HideInInspector] public CinemachineVirtualCamera examineCamera;
    private FirstPersonController _player;
    [HideInInspector] public Light examineLight;

    [HideInInspector] public bool isExamining;
    [HideInInspector] public Transform target;
    private const float YRotationSpeed = 5;
    private const float XRotationSpeed = 5;
    private float _initialRootX;
    private float _initialRootY;
    
    private PlayerExamineObjectController _examineObjectController;
    private InteractableObject _intObj;

    private void Awake()
    {
        examineLight = GameObject.Find("ExamineObjectLight")?.GetComponent<Light>();
        _player = FindObjectOfType<FirstPersonController>();
        _examineObjectController = FindObjectOfType<PlayerExamineObjectController>();
        target = GameObject.Find("ExaminationTarget")?.transform;
        playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        examineCamera = GameObject.Find("ExamineObjectCamera").GetComponent<CinemachineVirtualCamera>();
        _intObj = GetComponent<InteractableObject>();
        _intObj.onInteraction.AddListener(() => _examineObjectController.OnExaminationResetVectorValues());
    }

    private void Start()
    {
        if (examineLight != null)
        {
            examineLight.enabled = false;
        }
    }

    private void Update()
    {
        if (_examineObjectController == null) return;

        if (_examineObjectController.isExaminingObject)
        {
            if (!isExamining)
            {
                if (_examineObjectController.objectToExamine != null)
                {
                    _examineObjectController.originalRotation = _examineObjectController.objectToExamine.transform.rotation;
                }
                playerCamera.Priority = 0;
                examineCamera.Priority = 5;
                isExamining = true;
                if (examineLight != null)
                {
                    examineLight.enabled = true;
                }
            }
        }
        else if (isExamining) // Add this else-if to reset the state when not examining
        {
            isExamining = false;
            if (examineLight != null)
            {
                examineLight.enabled = false;
            }
            playerCamera.Priority = 5;
            examineCamera.Priority = 0;
        }

        if (isExamining)
        {
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                if (_examineObjectController.objectToExamine != null)
                {
                    _examineObjectController.objectToExamine.transform.Rotate(Vector3.up, -mouseX, Space.World);
                    _examineObjectController.objectToExamine.transform.Rotate(Vector3.right, mouseY, Space.World);

                    // Update the examine camera rotation to match the object's rotation
                    examineCamera.transform.rotation = _examineObjectController.objectToExamine.transform.rotation;
                }
            }

            if (Input.GetKeyDown(InGameSettingsManager.Instance.itemExaminationInteractionKey) 
                && _examineObjectController.objectToExamine != null)
            {
                _examineObjectController.ExitExamineMode();
            }
        }
    }
}