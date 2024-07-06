using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FadeUI : MonoBehaviour
{
    public float fadeDuration; 

    public void FadeText(TMP_Text tmpText)
    {
        if (tmpText != null)
        {
            StartCoroutine(FadeOutTMP(tmpText));
        }
    }

    public void FadeImage(Image image)
    {
        if (image != null)
        {
            StartCoroutine(FadeOutImage(image));
        }
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup)
    {
        float startAlpha = canvasGroup.alpha;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
            progress += rate * Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutTMP(TMP_Text tmpText)
    {
        float startAlpha = tmpText.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = tmpText.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 0, progress);
            tmpText.color = tmpColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }

        Color finalColor = tmpText.color;
        finalColor.a = 0;
        tmpText.color = finalColor;
        tmpText.gameObject.SetActive(false); 
    }

    private IEnumerator FadeOutImage(Image image)
    {
        float startAlpha = image.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color imgColor = image.color;
            imgColor.a = Mathf.Lerp(startAlpha, 0, progress);
            image.color = imgColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = 0;
        image.color = finalColor;
        image.gameObject.SetActive(false); 
    }
}
