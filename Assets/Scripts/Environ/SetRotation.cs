using UnityEngine;

public class SetRotation : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        transform.localRotation = target.localRotation;
    }
}
