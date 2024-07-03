using System.Collections;
using UnityEngine;

public class EnvironmentalSoundController : MonoBehaviour
{
    private static EnvironmentalSoundController _instance;
    private AudioSource _audioSource;

    public static EnvironmentalSoundController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnvironmentalSoundController>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EnvironmentalSoundController");
                    _instance = obj.AddComponent<EnvironmentalSoundController>();
                }
                DontDestroyOnLoad(_instance.gameObject);
                _instance._audioSource = _instance.GetComponent<AudioSource>();
                if (_instance._audioSource == null)
                {
                    _instance._audioSource = _instance.gameObject.AddComponent<AudioSource>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private bool IsSourcePlaying() => _audioSource.isPlaying;

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        transform.position = position;

        _audioSource.PlayOneShot(clip);
    }
}