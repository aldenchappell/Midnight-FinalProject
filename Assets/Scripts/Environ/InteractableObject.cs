using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class InteractableObject : MonoBehaviour
{
    public UnityEvent onInteraction;

    public Sprite interactionIcon;

    public Vector2 interactableIconSize;
    
    [HideInInspector] public int interactableID;

    [HideInInspector] public MouseHighlight mouseHighlight;

    private void Start()
    {
        interactableID = Random.Range(0,999999);
        
        mouseHighlight = GetComponent<MouseHighlight>();
    }

}