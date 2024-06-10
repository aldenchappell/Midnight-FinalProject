using System;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private bool isPhotoBoardCam = false;
    public Transform targetTransform;
    public Vector3 offset;

    private void Awake()
    {
        if (isPhotoBoardCam)
            targetTransform = GameObject.Find("CamPosition").transform;
    }

    private void Update()
    {
        transform.position = targetTransform.position + offset;
    }
}
