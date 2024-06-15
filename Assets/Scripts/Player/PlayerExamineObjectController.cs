using System.Collections;
using StarterAssets;
using UnityEngine;

public class PlayerExamineObjectController : MonoBehaviour
{
    public GameObject objectToExamine;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public bool isExaminingObject = false;
    public bool canExamine = true;
    private FirstPersonController _fpController;
    private Cinemachine.CinemachineVirtualCamera examineCamera;
    private Light _examineLight;
    private void Awake()
    {
        _fpController = FindObjectOfType<FirstPersonController>();
        examineCamera = GameObject.Find("ExamineObjectCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        _examineLight = GameObject.Find("ExamineObjectLight")?.GetComponent<Light>();
    }

    private void Start()
    {
        if (_examineLight != null)
        {
            _examineLight.enabled = false;
        }
    }
    
    public void StartExamination(GameObject examinableObj)
    {
        if (!objectToExamine.GetComponent<ExaminableObject>().isExamining)
        {
            objectToExamine = examinableObj;
            isExaminingObject = true;

            originalPosition = examinableObj.transform.position;
            originalRotation = examinableObj.transform.rotation;

            // Position the object at the examination target
            examinableObj.transform.position = FindObjectOfType<ExaminableObject>().target.position;

            // Ensure the camera looks at the object after it has moved
            examineCamera.transform.position = examinableObj.transform.position - examineCamera.transform.forward * 0.5f; // Adjust the offset as needed
            examineCamera.transform.LookAt(examinableObj.transform);

            _fpController.ToggleCanMove();
            _fpController.canRotate = false;

            // Set camera to look at the object
            examineCamera.Follow = examinableObj.transform;
            examineCamera.LookAt = examinableObj.transform;

            StartCoroutine(ExaminationCooldown());
            
            if (_examineLight != null)
            {
                _examineLight.enabled = true;
            }
        }
        
    }

    public void ExitExamineMode()
    {
        if (isExaminingObject && canExamine)
        {
            objectToExamine.transform.position = originalPosition;
            objectToExamine.transform.rotation = originalRotation;

            objectToExamine = null;
            isExaminingObject = false;

            _fpController.ToggleCanMove();
            _fpController.canRotate = true;

            var examinableObject = FindObjectOfType<ExaminableObject>();
            examinableObject.playerCamera.Priority = 5;
            examinableObject.examineCamera.Priority = 0;
            examinableObject.isExamining = false;
            
            examineCamera.m_LookAt = null;
            examineCamera.m_Follow = null;
            if (_examineLight != null)
            {
                _examineLight.enabled = false;
            }
        }
    }

    private IEnumerator ExaminationCooldown()
    {
        canExamine = false;
        yield return new WaitForSeconds(1.0f);
        canExamine = true;
    }

    public void OnExaminationResetVectorValues()
    {
        originalRotation = Quaternion.identity;
        originalPosition = Vector3.zero;
        isExaminingObject = false;
    }
}
