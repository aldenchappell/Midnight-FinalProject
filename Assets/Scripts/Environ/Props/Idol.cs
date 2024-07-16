using UnityEngine;

public class Idol : MonoBehaviour
{
    [SerializeField] private AudioClip idolCollectionClip;
    [SerializeField] private SO_Idol idol;
    private void Awake()
    {
        if (idol.collected)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        idol.collected = false;

        InteractableObject interactableObject = GetComponent<InteractableObject>();
        PlayerProgressionController progressController = FindObjectOfType<PlayerProgressionController>();
        if (interactableObject != null && progressController != null)
        {
            interactableObject.onInteraction.AddListener(() => progressController.CollectIdol(idol));
            interactableObject.onInteraction.AddListener(() => 
                EnvironmentalSoundController.Instance.PlaySound(idolCollectionClip, transform.position));
            interactableObject.onInteraction.AddListener(() => Destroy(gameObject));
        }
    }
    
}
