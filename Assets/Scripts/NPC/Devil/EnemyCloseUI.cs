using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCloseUI : MonoBehaviour
{
    [SerializeField] private Image enemyCloseImage;
    [SerializeField] private float maximumDistanceThreshold = 15.0f;
    [SerializeField] private GameObject demon;

    private PlayerDeathController _deathController;
    private bool _isFadingOut = false;

    private void Awake()
    {
        _deathController = GetComponent<PlayerDeathController>();
    }

    private void Start()
    {
        enemyCloseImage.enabled = false;
    }

    void Update()
    {
        if (demon == null || _deathController == null) return;

        float distanceToDemon = Vector3.Distance(transform.position, demon.transform.position);

        if (distanceToDemon <= maximumDistanceThreshold && !_deathController.isDead)
        {
            HandleEnemyCloseImageTransparency(distanceToDemon);
            enemyCloseImage.enabled = true;
            _isFadingOut = false;
        }
        else
        {
            if (!_isFadingOut)
            {
                StartCoroutine(FadeOutImage(enemyCloseImage));
                _isFadingOut = true;
            }
        }
    }

    private void HandleEnemyCloseImageTransparency(float distanceToPlayer)
    {
        float realDistance = Mathf.Clamp01(1 - (distanceToPlayer / maximumDistanceThreshold));
        Color color = enemyCloseImage.color;
        color.a = Mathf.Lerp(0, 1, realDistance);
        enemyCloseImage.color = color;
    }

    private IEnumerator FadeOutImage(Image image)
    {
        float startAlpha = image.color.a;
        float duration = .5f; 
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Color color = image.color;
            color.a = Mathf.Lerp(startAlpha, 0, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        image.enabled = false;
    }
}
