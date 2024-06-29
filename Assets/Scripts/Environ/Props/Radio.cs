using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour
{
    private InteractableObject _intObj;
    private AudioSource _audio;
    private bool _playing;
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        //_intObj.onInteraction.AddListener(ToggleRadio);
    }

    public void ToggleRadio()
    {
        if (_audio.isPlaying)
        {
            _audio.volume = 0f;
        }
        else
        {
            _audio.volume = 1f;
        }
    }
}
