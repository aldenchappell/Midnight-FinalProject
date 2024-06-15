using Cinemachine;
using StarterAssets;
using UnityEngine;

public class ExaminableObject : MonoBehaviour
{
    [HideInInspector] public CinemachineVirtualCamera playerCamera;
    [HideInInspector] public CinemachineVirtualCamera examineCamera;
    private FirstPersonController _player;

    [HideInInspector] public bool isExamining;
    [HideInInspector] public Transform target;
    
    private PlayerExamineObjectController _examineObjectController;
    private InteractableObject _intObj;

    private void Awake()
    {
        
        _player = FindObjectOfType<FirstPersonController>();
        _examineObjectController = FindObjectOfType<PlayerExamineObjectController>();
        target = GameObject.Find("ExaminationTarget")?.transform;
        playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        examineCamera = GameObject.Find("ExamineObjectCamera").GetComponent<CinemachineVirtualCamera>();
        _intObj = GetComponent<InteractableObject>();
        if(_intObj != null)
            _intObj.onInteraction.AddListener(() => _examineObjectController.OnExaminationResetVectorValues());
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
                examineCamera.LookAt = _examineObjectController.objectToExamine.transform;

                isExamining = true;
                
            }
        }
        else if (isExamining)
        {
            isExamining = false;

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
                }
            }

            if (Input.GetKeyDown(InGameSettingsManager.Instance.exitInteractionKey) 
                && _examineObjectController.objectToExamine != null)
            {
                _examineObjectController.ExitExamineMode();
            }
        }
    }
}