using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class ExaminableObject : InteractableObject
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

    public UnityEvent onExamination;
    private PlayerExamineObjectController _examineObjectController;
    
    private void Start()
    {
        examineLight = GameObject.Find("ExamineObjectLight")?.GetComponent<Light>();
        if (examineLight != null)
        {
            examineLight.enabled = false;
        }
        _player = FindObjectOfType<FirstPersonController>();
        _examineObjectController = FindObjectOfType<PlayerExamineObjectController>();
        target = GameObject.Find("ExaminationTarget")?.transform;
        playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        examineCamera = GameObject.Find("ExamineObjectCamera").GetComponent<CinemachineVirtualCamera>();
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