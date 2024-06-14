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

    private void Awake()
    {
        _fpController = FindObjectOfType<FirstPersonController>();
    }

    public void StartExamination(GameObject examinableObj)
    {
        objectToExamine = examinableObj;
        isExaminingObject = true;

        originalPosition = examinableObj.transform.position;
        originalRotation = examinableObj.transform.rotation;

        examinableObj.transform.position = FindObjectOfType<ExaminableObject>().target.position;
    
        _fpController.ToggleCanMove();
        _fpController.canRotate = false;

        StartCoroutine(ExaminationCooldown());
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
            if (examinableObject.examineLight != null)
            {
                examinableObject.examineLight.enabled = false;
            }
        }
    }

    private IEnumerator ExaminationCooldown()
    {
        canExamine = false;
        yield return new WaitForSeconds(1.0f);
        canExamine = true;
    }
}