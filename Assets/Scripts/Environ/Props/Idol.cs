using UnityEngine;

public class Idol : MonoBehaviour
{
    [SerializeField] private AudioClip idolCollectionClip;
    
    private void Awake()
    {
        InteractableObject interactableObject = GetComponent<InteractableObject>();
        PlayerProgressionController progressController = FindObjectOfType<PlayerProgressionController>();
        if (interactableObject != null && progressController != null)
        {
            interactableObject.onInteraction.AddListener(progressController.CollectIdol);
            interactableObject.onInteraction.AddListener(() => 
                EnvironmentalSoundController.Instance.PlaySound(idolCollectionClip, transform.position));
            interactableObject.onInteraction.AddListener(() => Destroy(gameObject));
        }
    }
}
