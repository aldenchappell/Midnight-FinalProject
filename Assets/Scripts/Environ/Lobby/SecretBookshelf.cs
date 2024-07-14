using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
        Renderer parentRenderer = GetComponent<Renderer>();

        //add the parent object to the renderer list 
        Renderer[] allRenderers = new Renderer[childRenderers.Length + 1];
        allRenderers[0] = parentRenderer;
        Array.Copy(childRenderers, 0, allRenderers, 1, childRenderers.Length);

        //set rendering mode to fade
        foreach (Renderer r in allRenderers)
        {
            Material material = r.material;
            material.SetRenderingMode(RenderingMode.Fade);
        }

        float duration = 4.0f;
        float elapsedTime = 0f;

        //get the initial colors and alphas
        Color[] startingColors = new Color[allRenderers.Length];
        float[] startingAlphas = new float[allRenderers.Length];

        for (int i = 0; i < allRenderers.Length; i++)
        {
            startingColors[i] = allRenderers[i].material.color;
            startingAlphas[i] = startingColors[i].a;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            for (int i = 0; i < allRenderers.Length; i++)
            {
                Color newColor = new Color(startingColors[i].r, startingColors[i].g, startingColors[i].b, newAlpha);
                allRenderers[i].material.color = newColor;
            }

            yield return null;
        }

        for (int i = 0; i < allRenderers.Length; i++)
        {
            Color finalColor = new Color(startingColors[i].r, startingColors[i].g, startingColors[i].b, 0f);
            allRenderers[i].material.color = finalColor;
        }

        //deactivate after all renderers have faded out
        gameObject.SetActive(false);
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
