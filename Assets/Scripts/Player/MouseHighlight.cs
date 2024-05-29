using System;
using UnityEngine;

public class MouseHighlight : MonoBehaviour
{
    private Color[] startColors;
    public Renderer[] renderers;

    private void Awake()
    {
        
    }

    private void Start()
    {
        // Initialize the array
        renderers = GetComponents<Renderer>();
        startColors = new Color[renderers.Length];
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
        for (int index = 0; index < renderers.Length; ++index)
            startColors[index] = renderers[index].material.color;
    }

    public void ResetColor()
    {
        for (int index = 0; index < renderers.Length; ++index)
            renderers[index].material.color = startColors[index];
    }

    public void ChangeColor(Color shade)
    {
        for (int index = 0; index < renderers.Length; ++index)
            renderers[index].material.color = shade;
    }
}
