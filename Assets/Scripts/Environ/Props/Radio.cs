using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour
{
    private InteractableObject _intObj;
    private AudioSource _audio;
    private bool _playing = true;
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        //_intObj.onInteraction.AddListener(ToggleRadio);
    }

    public void ToggleRadio()
    {
        _audio.volume = !_playing ? 0.41f : 0f;

        _playing = !_playing;
    }
}
