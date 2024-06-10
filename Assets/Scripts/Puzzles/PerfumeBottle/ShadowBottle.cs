using UnityEngine;

public class ShadowBottle : MonoBehaviour
{
    private void Start()
    {
        var interactable = GetComponent<InteractableObject>();
        interactable.onPlaceObject.AddListener(LevelCompletionManager.Instance.OnKeySpawn);
    }
}