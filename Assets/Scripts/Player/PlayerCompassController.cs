using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCompassController : MonoBehaviour
{
    [SerializeField] private RectTransform compassBarTransform;
    [SerializeField] private GameObject markerPrefab; // Prefab for the marker UI element

    private List<RectTransform> _objectiveMarkerTransforms = new List<RectTransform>();
    private List<Transform> _objectiveObjectTransforms = new List<Transform>();
    private List<Puzzle> _puzzlesWithListeners = new List<Puzzle>();

    public Transform cameraObjectTransform;

    private void Start()
    {
        InitializeMarkers();
    }

    private void Update()
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
                SetCompassMarkerPosition(_objectiveMarkerTransforms[i], _objectiveObjectTransforms[i].position);
            }
        }
    }

    private void InitializeMarkers()
    {
        CompassMarker[] compassObjects = FindObjectsOfType<CompassMarker>();

        foreach (var compassMarker in compassObjects)
        {
            if (compassMarker != null)
            {
                // Create a new marker UI element as a child of the compass bar
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

                // Find the puzzle component on the same object
                Puzzle puzzle = compassMarker.GetComponent<Puzzle>();
                if (puzzle != null && !_puzzlesWithListeners.Contains(puzzle))
                {
                    puzzle.onPuzzleCompletion.AddListener(() => RemoveMarker(compassMarker.transform));
                    _puzzlesWithListeners.Add(puzzle);
                    //Debug.Log("added listener for removing marker on the puzzle " + puzzle.name);
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
        Vector3 dirToTarget = position - cameraObjectTransform.position;

        float angle = Vector2.SignedAngle(new Vector2(dirToTarget.x, dirToTarget.z),
            new Vector2(cameraObjectTransform.forward.x, cameraObjectTransform.forward.z));
        float compassXPos = Mathf.Clamp(
            2 * angle / cameraObjectTransform.GetComponent<Camera>().fieldOfView,
            -1,
            1);
        markerTransform.anchoredPosition = new Vector2(compassBarTransform.rect.width / 2 * compassXPos, 0);
    }
}
