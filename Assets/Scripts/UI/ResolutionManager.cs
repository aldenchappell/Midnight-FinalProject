using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private Resolution[] _resolutions;

    private void Start()
    {
        _resolutions = Screen.resolutions;
        ApplySavedResolution();
    }

    private void ApplySavedResolution()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", _resolutions.Length - 1);
        if (savedResolutionIndex >= 0 && savedResolutionIndex < _resolutions.Length)
        {
            Resolution resolution = _resolutions[savedResolutionIndex];
            Debug.Log("Applying Saved Resolution: " + resolution.width + " x " + resolution.height);
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        else
        {
            Debug.LogError("Invalid resolution index in PlayerPrefs: " + savedResolutionIndex);
        }
    }
}