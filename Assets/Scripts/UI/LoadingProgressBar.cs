using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    private Image _loadingImage;
    private void Awake()
    {
        _loadingImage = GetComponent<Image>();
    }

    private void Update()
    {
        _loadingImage.fillAmount = Loader.GetLoadingProgress();
    }
}
