using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCompassController : MonoBehaviour
{
    [SerializeField] private RectTransform compassBarTransform;
    [SerializeField] private GameObject markerPrefab; // Prefab for the marker UI element

    public List<RectTransform> objectiveMarkerTransforms = new List<RectTransform>();
    public List<Transform> objectiveObjectTransforms = new List<Transform>();

    public Transform cameraObjectTransform;

    private void Start()
    {
        // Find all GameObjects with the tag "Compass"
        CompassMarker[] compassObjects = GameObject.FindObjectsOfType<CompassMarker>();

        foreach (var obj in compassObjects)
        {
            // Ensure the GameObject has a CompassMarker component
            CompassMarker compassMarker = obj.GetComponent<CompassMarker>();
            if (compassMarker != null)
            {
                // Create a new marker UI element as a child of the compass bar
                GameObject marker = Instantiate(markerPrefab, compassBarTransform);
                RectTransform markerRectTransform = marker.GetComponent<RectTransform>();

                // Set the custom image from the ScriptableObject
                Image markerImage = marker.GetComponent<Image>();
                if (markerImage != null && compassMarker.compassMarkerSprite != null)
                {
                    markerImage.sprite = compassMarker.compassMarkerSprite.compassMarkerSprite;
                }

                // Add the marker and the corresponding object's transform to the lists
                objectiveMarkerTransforms.Add(markerRectTransform);
                objectiveObjectTransforms.Add(obj.transform);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < objectiveMarkerTransforms.Count; i++)
        {
            SetCompassMarkerPosition(objectiveMarkerTransforms[i], objectiveObjectTransforms[i].position);
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
