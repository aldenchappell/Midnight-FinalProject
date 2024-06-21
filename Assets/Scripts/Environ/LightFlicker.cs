using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    private Light _light;
    [SerializeField] private float[] flickerTime;
    public bool shouldFlicker = false;
    private Coroutine _flickerCoroutine;

    public AudioClip flickerSound;
    public static AudioSource AudioSource;

    private Renderer _sconceRenderer;
    [SerializeField] private Material sconceOnMat, sconceOffMat;
    private GameObject _enemy;

    private void Awake()
    {
        _light = GetComponent<Light>();
        flickerTime = new float[2] { 0.5f, 1.5f };
        _enemy = FindObjectOfType<EnemyFlickerLightsInVicinity>()?.gameObject;

        _sconceRenderer = GetComponentInParent<Renderer>();
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
            SetLightState(true); // Ensure light is turned on when flickering stops
        }
    }

    private IEnumerator FlickerLights()
    {
        while (shouldFlicker)
        {
            SetLightState(false);
            yield return new WaitForSeconds(Random.Range(flickerTime[0], flickerTime[1]));
            SetLightState(true);
            yield return new WaitForSeconds(Random.Range(flickerTime[0], flickerTime[1]));
        }
    }

    private void SetLightState(bool state)
    {
        _light.enabled = state;
        _sconceRenderer.material = state ? sconceOnMat : sconceOffMat;
    }
}