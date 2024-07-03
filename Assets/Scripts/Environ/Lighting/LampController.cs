using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LampController : MonoBehaviour
{
    private AudioSource _source;
    [SerializeField] private AudioClip pullLampSound;

    private List<Lamp> _lamps = new List<Lamp>();

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "LOBBY")
        {
            Init();

            if (LevelCompletionManager.Instance.hasCompletedLobby)
            {
                ToggleAllLamps(true);
            }
            else
            {
                ToggleAllLamps(false);
            }
        }
    }

    private void Init()
    {
        GameObject[] foundLamps = GameObject.FindGameObjectsWithTag("Lamp");

        _lamps.Clear();

        foreach (GameObject lampObject in foundLamps)
        {
            Lamp lampComponent = lampObject.GetComponent<Lamp>();
            if (lampComponent != null)
            {
                lampComponent.SetLampController(this);
                _lamps.Add(lampComponent);
            }
        }
    }

    public void HandleLamp(Lamp lamp)
    {
        MoveToLamp(lamp);
        PlaySound();
        ToggleLights(lamp);
    }

    private void PlaySound()
    {
        if (!_source.isPlaying && pullLampSound != null)
        {
            _source.PlayOneShot(pullLampSound);
        }
    }

    private void ToggleLights(Lamp lamp)
    {
        lamp.on = !lamp.on;
        Light lampLight = lamp.GetComponentInChildren<Light>();
        Renderer renderer = lamp.GetComponentInChildren<Renderer>();
        if (lamp.on)
        {
            renderer.material.EnableKeyword("_EMISSION");
            lampLight.enabled = true;
        }
        else
        {
            renderer.material.DisableKeyword("_EMISSION");
            lampLight.enabled = false;
        }
    }

    private void MoveToLamp(Lamp lamp)
    {
        transform.position = lamp.transform.position;
    }

    private void ToggleAllLamps(bool enable)
    {
        foreach (var lamp in _lamps)
        {
            lamp.on = enable;
            Light lampLight = lamp.GetComponentInChildren<Light>();
            Renderer renderer = lamp.GetComponentInChildren<Renderer>();
            if (enable)
            {
                renderer.material.EnableKeyword("_EMISSION");
                lampLight.enabled = true;
            }
            else
            {
                renderer.material.DisableKeyword("_EMISSION");
                lampLight.enabled = false;
            }
        }
    }

    public void PowerOnLamps()
    {
        ToggleAllLamps(true);
    }
}
