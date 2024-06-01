using System;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private Color[] _startColors;
    private Renderer[] _renderers;
    
    private void Start()
    {
        // Initialize the array
        _renderers = GetComponents<Renderer>();
        _startColors = new Color[_renderers.Length];
        SetColor();
    }


    private void Update()
    {
        if (!Input.GetButtonDown("Fire1"))
            return;
        ResetColor();
    }

    public void SetColor()
    {
        for (int index = 0; index < _renderers.Length; ++index)
            _startColors[index] = _renderers[index].material.color;
    }

    public void ResetColor()
    {
        for (int index = 0; index < _renderers.Length; ++index)
            _renderers[index].material.color = _startColors[index];
    }

    public void ChangeColor(Color shade)
    {
        for (int index = 0; index < _renderers.Length; ++index)
            _renderers[index].material.color = shade;
    }
}
