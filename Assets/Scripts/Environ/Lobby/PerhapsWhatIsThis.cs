using UnityEngine;
using System.Collections;

public class PerhapsWhatIsThis : MonoBehaviour
{
    [SerializeField] private InteractableObject[] bababooeys;
    [SerializeField] private AudioClip[] bababooeyClips;
    private const float AnimationDuration = 0.1f; 
    private const float AnimationDistance = 0.75f;

    private void Awake()
    {
        AssignAudioClipToBabaBooey();
    }

    private void AssignAudioClipToBabaBooey()
    {
        for (int i = 0; i < bababooeys.Length; i++)
        {
            int index = i;
            bababooeys[i].onInteraction.AddListener(() => PlayBababooeySound(bababooeys[index], bababooeyClips[index]));
        }
    }

    public void PlayBababooeySound(InteractableObject key, AudioClip note)
    {
        EnvironmentalSoundController.Instance.PlaySound(note, transform.position);
        StartCoroutine(PlayArtificialKeyAnimation(key.transform));
    }

    private IEnumerator PlayArtificialKeyAnimation(Transform keyTransform)
    {
        Vector3 originalPosition = keyTransform.localPosition;
        Vector3 pressedPosition = originalPosition + Vector3.down * AnimationDistance;

        //key down
        float elapsedTime = 0f;
        while (elapsedTime < AnimationDuration)
        {
            keyTransform.localPosition = Vector3.Lerp(originalPosition, pressedPosition, elapsedTime / AnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        keyTransform.localPosition = pressedPosition;

        //key up
        elapsedTime = 0f;
        while (elapsedTime < AnimationDuration)
        {
            keyTransform.localPosition = Vector3.Lerp(pressedPosition, originalPosition, elapsedTime / AnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        keyTransform.localPosition = originalPosition;
    }
}
