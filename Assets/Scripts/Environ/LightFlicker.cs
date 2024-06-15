using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    private Light _light;
    [SerializeField] private float[] flickerTime;
    public bool shouldFlicker = false;
    private Coroutine _flickerCoroutine;

    public AudioClip flickerSound;
    public static AudioSource AudioSource;


    private GameObject _enemy;
    private void Awake()
    {
        _light = GetComponent<Light>();
        flickerTime = new float[2] { 0.5f, 1.5f };
        _enemy = FindObjectOfType<EnemyFlickerLightsInVicinity>()?.gameObject;
        
        
        GameObject lightAudioManager = GameObject.Find("LightAudioManager");
        if (lightAudioManager != null)
        {
            AudioSource = lightAudioManager.GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (shouldFlicker && _enemy != null)
        {
            StartFlickering();
        }
    }

    public void StartFlickering()
    {
        if (_flickerCoroutine == null && _enemy != null)
        {
            _flickerCoroutine = StartCoroutine(FlickerLights());
        }
    }

    public void StopFlickering()
    {
        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
            _flickerCoroutine = null;
            _light.enabled = true; 
        }
    }

    private IEnumerator FlickerLights()
    {
        while (shouldFlicker)
        {
            _light.enabled = false;
            
            if (AudioSource != null && !AudioSource.isPlaying)
            {
                AudioSource.transform.position = transform.position; 
                AudioSource.PlayOneShot(flickerSound, 3.0f);
            }
            yield return new WaitForSeconds(Random.Range(flickerTime[0], flickerTime[1]));
            _light.enabled = true;
            yield return new WaitForSeconds(Random.Range(flickerTime[0], flickerTime[1]));
        }
    }
}
