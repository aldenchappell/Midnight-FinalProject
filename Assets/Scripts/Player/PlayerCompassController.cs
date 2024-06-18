using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCompassController : MonoBehaviour
{
    [SerializeField] private RectTransform compassBarTransform; //compass bar
    [SerializeField] private GameObject markerPrefab; //prefab that spawns to show marker on compass
    private const float MaxDistanceToShowMarker = 12.5f; //maximum distance from the player to show marker
    private const float MarkerDetectionAngle = 90f; //angle from the camera to look for markers
    private const float CompassFadeDuration = 1.25f; //how long it takes for the compass to fade

    private readonly List<RectTransform> _objectiveMarkerTransforms = new List<RectTransform>();
    private readonly List<Transform> _objectiveObjectTransforms = new List<Transform>();
    private readonly List<Puzzle> _puzzles = new List<Puzzle>();

    private Transform _cameraObjectTransform;

    private float _lastMarkerVisibleTime;
    private const float MaxTimeWithNoMarkersFound = 1.0f;
    private bool _isFadingOut;
    private float _fadeStartTime;

    private void Awake()
    {
        if (Camera.main != null) _cameraObjectTransform = Camera.main.transform;
    }

    private void Start()
    {
        if (InGameSettingsManager.Instance.enableCompass)
        {
            InitializeMarkers();
            SetCompassAlpha(1);
        }
        else
        {
            SetCompassAlpha(0);
        }
    }

    private void Update()
    {
        bool anyMarkersVisible = false;

        if (InGameSettingsManager.Instance.enableCompass)
        {
            for (int i = _objectiveMarkerTransforms.Count - 1; i >= 0; i--)
            {
                if (_objectiveObjectTransforms[i] == null)
                {
                    Destroy(_objectiveMarkerTransforms[i].gameObject);
                    _objectiveMarkerTransforms.RemoveAt(i);
                    _objectiveObjectTransforms.RemoveAt(i);
                }
                else
                {
                    float distanceToMarker = Vector3.Distance(_cameraObjectTransform.position, _objectiveObjectTransforms[i].position);
                    Vector3 directionToMarker = (_objectiveObjectTransforms[i].position - _cameraObjectTransform.position).normalized;
                    float angleToMarker = Vector3.Angle(directionToMarker, _cameraObjectTransform.forward);

                    if (distanceToMarker <= MaxDistanceToShowMarker && angleToMarker <= MarkerDetectionAngle / 2)
                    {
                        _objectiveMarkerTransforms[i].gameObject.SetActive(true);
                        SetCompassMarkerPosition(_objectiveMarkerTransforms[i], _objectiveObjectTransforms[i].position);
                        anyMarkersVisible = true;
                    }
                    else
                    {
                        _objectiveMarkerTransforms[i].gameObject.SetActive(false);
                    }
                }
            }

            if (anyMarkersVisible)
            {
                _lastMarkerVisibleTime = Time.time;
                if (_isFadingOut)
                {
                    _isFadingOut = false;
                    SetCompassAlpha(1);
                }
            }
            else if (Time.time - _lastMarkerVisibleTime > MaxTimeWithNoMarkersFound)
            {
                if (!_isFadingOut)
                {
                    _isFadingOut = true;
                    _fadeStartTime = Time.time;
                }
                float elapsedTime = Time.time - _fadeStartTime;
                if (elapsedTime < CompassFadeDuration)
                {
                    float newAlpha = Mathf.Lerp(1, 0, elapsedTime / CompassFadeDuration);
                    SetCompassAlpha(newAlpha);
                }
                else
                {
                    SetCompassAlpha(0);
                }
            }
        }
        else
        {
            SetCompassAlpha(0);
        }
    }

    private void InitializeMarkers()
    {
        CompassMarker[] compassObjects = FindObjectsOfType<CompassMarker>();

        foreach (var compassMarker in compassObjects)
        {
            if (compassMarker != null)
            {
                GameObject marker = Instantiate(markerPrefab, compassBarTransform);
                RectTransform markerRectTransform = marker.GetComponent<RectTransform>();

                Image markerImage = marker.GetComponent<Image>();
                if (markerImage != null && compassMarker.compassMarkerSprite != null)
                {
                    markerImage.sprite = compassMarker.compassMarkerSprite.compassMarkerSprite;
                    markerImage.rectTransform.sizeDelta = compassMarker.compassMarkerSprite.compassMarkerSpriteSize;
                }

                _objectiveMarkerTransforms.Add(markerRectTransform);
                _objectiveObjectTransforms.Add(compassMarker.transform);

                Puzzle puzzle = compassMarker.GetComponent<Puzzle>();
                if (puzzle != null && !_puzzles.Contains(puzzle))
                {
                    puzzle.onPuzzleCompletion.AddListener(() => RemoveMarker(compassMarker.transform));
                    _puzzles.Add(puzzle);
                }
            }
        }
    }

    public void RemoveMarker(Transform target)
    {
        int index = _objectiveObjectTransforms.IndexOf(target);
        if (index != -1)
        {
            Destroy(_objectiveMarkerTransforms[index].gameObject);
            _objectiveMarkerTransforms.RemoveAt(index);
            _objectiveObjectTransforms.RemoveAt(index);
        }
    }

    private void SetCompassMarkerPosition(RectTransform markerTransform, Vector3 position)
    {
        Vector3 dirToTarget = position - _cameraObjectTransform.position;

        float angle = Vector2.SignedAngle(new Vector2(dirToTarget.x, dirToTarget.z),
            new Vector2(_cameraObjectTransform.forward.x, _cameraObjectTransform.forward.z));
        float compassXPos = Mathf.Clamp(
            2 * angle / _cameraObjectTransform.GetComponent<Camera>().fieldOfView,
            -1,
            1);
        markerTransform.anchoredPosition = new Vector2(compassBarTransform.rect.width / 2 * compassXPos, 0);
    }

    private void SetCompassAlpha(float alpha)
    {
        Image compassImage = compassBarTransform.GetComponent<Image>();
        if (compassImage != null)
        {
            Color color = compassImage.color;
            color.a = alpha;
            compassImage.color = color;
        }

        foreach (RectTransform markerTransform in _objectiveMarkerTransforms)
        {
            Image markerImage = markerTransform.GetComponent<Image>();
            if (markerImage != null)
            {
                Color color = markerImage.color;
                color.a = alpha;
                markerImage.color = color;
            }
        }
    }
}