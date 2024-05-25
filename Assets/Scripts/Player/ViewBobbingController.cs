using System;
using UnityEngine;


[RequireComponent(typeof(FollowPosition))]
public class ViewBobbingController : MonoBehaviour
{
    [SerializeField] private float bobbingIntensity;
    [SerializeField] private float bobbingIntensityX;
    [SerializeField] private float bobbingEffectSpeed;

    private FollowPosition _followPosition;
    private Vector3 _originalOffset;
    private float _sinTime;

    private void Start()
    {
        _followPosition = GetComponent<FollowPosition>();
        _originalOffset = _followPosition.offset;
    }

    private void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0.0f, Input.GetAxis("Horizontal"));

        if (inputVector.magnitude > 0.0f)
        {
            _sinTime += Time.deltaTime * bobbingEffectSpeed;
        }
        else
        {
            _sinTime = 0.0f;
        }
        
        float sinAmountY = -Mathf.Abs(bobbingIntensity * Mathf.Sin(_sinTime));
        Vector3 sinAmountX = _followPosition.transform.right
                             * bobbingIntensity *
                             Mathf.Cos(_sinTime) *
                             bobbingIntensityX;

        _followPosition.offset = new Vector3
        {
            x = _originalOffset.x,
            y = _originalOffset.y + sinAmountY,
            z = _originalOffset.z
        };

        _followPosition.offset += sinAmountX;
    }
}
