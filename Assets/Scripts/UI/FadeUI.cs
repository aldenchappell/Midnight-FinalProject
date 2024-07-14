using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FadeUI : MonoBehaviour
{
    public float fadeDuration;

    public void FadeOutText(TMP_Text tmpText)
    {
        if (tmpText != null)
        {
            StartCoroutine(FadeOutTMP(tmpText));
        }
    }

    public void FadeInAndOutText(TMP_Text tmpText)
    {
        if (tmpText != null)
        {
            StartCoroutine(FadeInAndOutTMP(tmpText));
        }
    }

    public void FadeOutImage(Image image)
    {
        if (image != null)
        {
            StartCoroutine(FadeOutImageCoroutine(image));
        }
    }

    public void FadeInAndOutImage(Image image)
    {
        if (image != null)
        {
            StartCoroutine(FadeInAndOutImageCoroutine(image));
        }
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
    }

    private IEnumerator FadeInAndOutTMP(TMP_Text tmpText)
    {
        yield return StartCoroutine(FadeInTMP(tmpText));
        yield return StartCoroutine(FadeOutTMP(tmpText));
    }

    private IEnumerator FadeInTMP(TMP_Text tmpText)
    {
        float startAlpha = tmpText.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = tmpText.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 1, progress);
            tmpText.color = tmpColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }

        Color finalColor = tmpText.color;
        finalColor.a = 1;
        tmpText.color = finalColor;
    }

    private IEnumerator FadeOutImageCoroutine(Image image)
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
    }

    private IEnumerator FadeInAndOutImageCoroutine(Image image)
    {
        yield return StartCoroutine(FadeInImage(image));
        yield return StartCoroutine(FadeOutImageCoroutine(image));
    }

    private IEnumerator FadeInImage(Image image)
    {
        float startAlpha = image.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color imgColor = image.color;
            imgColor.a = Mathf.Lerp(startAlpha, 1, progress);
            image.color = imgColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = 1;
        image.color = finalColor;
    }

    public void ImageFadeIn(Image image)
    {
        StartCoroutine(FadeInImage(image));
    }
    
    public IEnumerator FadeEnemyCloseImage(Image image)
    {
        float fadeDuration = 0.1f; // Very fast fade duration
        float startAlpha = image.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        // Fade in
        while (progress < 1.0f)
        {
            Color imgColor = image.color;
            imgColor.a = Mathf.Lerp(startAlpha, 43f / 255f, progress);
            image.color = imgColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is clamped
        Color finalColor = image.color;
        finalColor.a = 43f / 255f;
        image.color = finalColor;

        // Fade out
        progress = 0.0f;
        while (progress < 1.0f)
        {
            Color imgColor = image.color;
            imgColor.a = Mathf.Lerp(43f / 255f, 0, progress);
            image.color = imgColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }

        finalColor.a = 0;
        image.color = finalColor;
    }
}
