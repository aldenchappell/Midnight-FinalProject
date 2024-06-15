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
}