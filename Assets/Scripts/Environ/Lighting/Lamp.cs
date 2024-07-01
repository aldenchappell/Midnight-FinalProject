using UnityEngine;
using UnityEngine.Events;

public class Lamp : MonoBehaviour
{
    public bool on = true;
    
    private LampController _lampController;

    [System.Serializable]
    public class LampInteractionEvent : UnityEvent<Lamp> { }
    

    private void Awake()
    {
        _lampController = FindObjectOfType<LampController>();
    }

    private void Start()
    {
        var onInteraction = GetComponent<InteractableObject>().onInteraction;
        var demonPatrolManager = FindObjectOfType<PatrolSystemManager>();
        if (onInteraction != null)
        {
            onInteraction.AddListener(() => _lampController.HandleLamp(this));
            if(demonPatrolManager)
                onInteraction.AddListener(() => demonPatrolManager.RaiseSuspicion = 5);
        }
        
    }

    public void SetLampController(LampController lampController)
    {
        _lampController = lampController;
    }
}