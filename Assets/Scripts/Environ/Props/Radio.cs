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
            _audio.Pause();
        }
        else
        {
            _audio.UnPause();
        }
    }
}
