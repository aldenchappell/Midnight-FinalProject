using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Slider slider;
    [SerializeField] private string volumeParameter;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float volumeMultiplier = 30f;

    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        if (value <= 0f)
        {
            mixer.SetFloat(volumeParameter, -80f); //muted
            volumeText.text = "Muted";
        }
        else
        {
            mixer.SetFloat(volumeParameter, Mathf.Log10(value) * volumeMultiplier);
            volumeText.text = value.ToString("f1");
        }

        InGameSettingsManager.Instance.SetVolume(volumeParameter, value);
    }
}
