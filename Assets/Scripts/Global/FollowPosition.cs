using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;

    private void Update()
    {
        transform.position = targetTransform.position + offset;
    }
}
