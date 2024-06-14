using UnityEngine;

public class ExaminationCameraController : MonoBehaviour
{
    public Transform playerCameraTransform; // Reference to the player's camera transform
    public Transform examinationTarget; // Target position where the camera will look at
    public Vector3 offset = new Vector3(0f, 1f, 0f); // Offset from the examination target
    public float smoothSpeed = 5f; // Smoothness of camera movement

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isExamining = false;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (isExamining)
        {
            // Move and rotate the camera towards the examination target
            Vector3 targetPosition = examinationTarget.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(examinationTarget);
        }
        else
        {
            // Reset the camera to its original position and rotation
            transform.position = Vector3.Lerp(transform.position, originalPosition, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, smoothSpeed * Time.deltaTime);
        }
    }

    public void StartExamination()
    {
        isExamining = true;
    }

    public void StopExamination()
    {
        isExamining = false;
    }
}