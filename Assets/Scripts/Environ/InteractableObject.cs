using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class InteractableObject : MonoBehaviour
{
    public UnityEvent onInteraction;

    public Sprite interactionIcon;

    public Vector2 interactableIconSize;
    
    [HideInInspector] public int interactableID;

    private void Start()
    {
        interactableID = Random.Range(0,999999);
    }
}