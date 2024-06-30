using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDoorExit : MonoBehaviour
{
    [SerializeField] private GameObject outroCutscene;
    [SerializeField] private Image textPanel;
    [SerializeField] private TMP_Text text;

    private Coroutine _textCoroutine;
    public void PlayOutroCutscene()
    {
        if (LevelCompletionManager.Instance.completedLevels.Count == 4)
        {
            outroCutscene.SetActive(true);
        }
        else
        {
            if (_textCoroutine != null)
            {
                StopCoroutine(_textCoroutine);
            }
            _textCoroutine = StartCoroutine(FadeOutText());
        }
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
        float time = 0f;
        Color panelColor = textPanel.color;
        Color textColor = text.color;

        textPanel.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        while (time < duration)
        {
            time += Time.deltaTime;
            float desiredAlpha = Mathf.Lerp(startingAlpha, endingAlpha, time / duration);

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