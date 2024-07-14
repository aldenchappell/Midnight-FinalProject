using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecretBookshelf : MonoBehaviour
{
    [SerializeField] private Image textPanel;
    [SerializeField] private TMP_Text text;
    [HideInInspector] public InteractableObject interactable;
    private CompassMarker _compassMarker;
    private Coroutine _textCoroutine;
    private bool _allowTextPrompt;

    private void Awake()
    {
        _compassMarker = GetComponent<CompassMarker>();
        interactable = GetComponent<InteractableObject>();
        
        if(LevelCompletionManager.Instance.allIdolsCollected)
            LevelCompletionManager.Instance.UnlockSecretRoom();
    }

    private void Start()
    {
        if (LevelCompletionManager.Instance.allIdolsCollected)
        {
            _compassMarker.enabled = true;
            interactable.onInteraction.AddListener(TriggerSecretBookshelf);
        }
        else
        {
            _compassMarker.enabled = false;
            FindObjectOfType<PlayerCompassController>().RemoveMarker(_compassMarker.transform);
            interactable.onInteraction.AddListener(FadeInAndOutText);
        }
    }

    public void UnlockBookshelfAccess()
    {
        LevelCompletionManager.Instance.UnlockSecretRoom();

        interactable.enabled = true;
        interactable.onInteraction.RemoveListener(FadeInAndOutText);
        interactable.onInteraction.AddListener(TriggerSecretBookshelf);

        Debug.Log("Bookshelf access granted.");
    }

    private void TriggerSecretBookshelf()
    {
        if (_textCoroutine != null)
        {
            StopCoroutine(_textCoroutine);
        }
        _textCoroutine = StartCoroutine(FadeBookshelf());
    }

    private IEnumerator FadeBookshelf()
    {
        GameObject secretDoorBookshelf = GameObject.Find("SecretDoorBookshelf");
        if (secretDoorBookshelf == null)
        {
            yield break;
        }

        Renderer bookshelfRenderer = secretDoorBookshelf.GetComponent<Renderer>();

        Color startingColor = bookshelfRenderer.material.color;
        float startingAlpha = startingColor.a;

        float duration = 4.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startingAlpha, 0f, elapsedTime / duration);
            Color newColor = new Color(startingColor.r, startingColor.g, startingColor.b, newAlpha);
            bookshelfRenderer.material.color = newColor;
            yield return null;
        }

        Color finalColor = new Color(startingColor.r, startingColor.g, startingColor.b, 0f);
        bookshelfRenderer.material.color = finalColor;

        secretDoorBookshelf.gameObject.SetActive(false);
    }

    private void FadeInAndOutText()
    {
        if (_textCoroutine != null)
        {
            StopCoroutine(_textCoroutine);
        }
        _textCoroutine = StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        float fadeDuration = 1.5f;
        float displayDuration = 3.0f; //how long the prompt shows on the screen

        //fade in
        yield return StartCoroutine(Fade(0, 1, fadeDuration));

        yield return new WaitForSeconds(displayDuration);

        //fade out
        yield return StartCoroutine(Fade(1, 0, fadeDuration));
    }

    private IEnumerator Fade(float startingAlpha, float endingAlpha, float duration)
    {
        var time = 0f;
        var panelColor = textPanel.color;
        var textColor = text.color;

        textPanel.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        //fading
        while (time < duration)
        {
            time += Time.deltaTime;
            var desiredAlpha = Mathf.Lerp(startingAlpha, endingAlpha, time / duration);

            panelColor.a = desiredAlpha;
            textColor.a = desiredAlpha;

            textPanel.color = panelColor;
            text.color = textColor;

            yield return null;
        }

        panelColor.a = endingAlpha;
        textColor.a = endingAlpha;
        textPanel.color = panelColor;
        text.color = textColor;

        if (endingAlpha <= 0f)
        {
            Debug.Log("fade completed. disabling panel.");
            textPanel.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }
    }
}
