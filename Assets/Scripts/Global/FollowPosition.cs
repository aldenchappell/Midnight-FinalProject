using System;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private bool isPhotoBoardCam = false;
    public Transform targetTransform;
    public Vector3 offset;
    [SerializeField] private bool enableRotation = false;
    [SerializeField] private bool enableRotationOffset;
    [SerializeField] private Quaternion rotationOffset;
    private void Awake()
    {
        if (isPhotoBoardCam)
            targetTransform = GameObject.Find("CamPosition").transform;
    }

    private void Update()
    {
        transform.position = targetTransform.position + offset;

        if (enableRotation)
        {
            transform.rotation = targetTransform.rotation;
        }
        
        if (enableRotationOffset)
        {
            transform.rotation = targetTransform.rotation * rotationOffset;
        }
    }
}
