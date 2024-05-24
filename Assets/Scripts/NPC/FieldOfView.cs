using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Material fieldOfViewMaterial;
    [SerializeField] private float visionRange;
    [SerializeField] private float visionAngle;
    [SerializeField] private LayerMask visionObstructingLayer;//layer with objects that obstruct the enemy view, like walls, for example
    [SerializeField] private int visionConeResolution = 120;//the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    private Mesh _visionConeMesh;
    private MeshFilter _meshFilter;

    void Start()
    {
        transform.AddComponent<MeshRenderer>().material = fieldOfViewMaterial;
        _meshFilter = transform.AddComponent<MeshFilter>();
        _visionConeMesh = new Mesh();
    }

    
    void Update()
    {
        DrawVisionCone();
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
	int[] triangles = new int[(visionConeResolution - 1) * 3];
    	Vector3[] vertices = new Vector3[visionConeResolution + 1];
        vertices[0] = Vector3.zero;
        float currentangle = -visionAngle / 2;
        float angleIncrement = visionAngle / (visionConeResolution - 1);
        float sine;
        float cosine;

        for (int i = 0; i < visionConeResolution; i++)
        {
            sine = Mathf.Sin(currentangle);
            cosine = Mathf.Cos(currentangle);
            Vector3 raycastDirection = (transform.forward * cosine) + (transform.right * sine);
            Vector3 vertForward = (Vector3.forward * cosine) + (Vector3.right * sine);
            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, visionRange, visionObstructingLayer))
            {
                vertices[i + 1] = vertForward * hit.distance;
            }
            else
            {
                vertices[i + 1] = vertForward * visionRange;
            }


            currentangle += angleIncrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        _visionConeMesh.Clear();
        _visionConeMesh.vertices = vertices;
        _visionConeMesh.triangles = triangles;
        _meshFilter.mesh = _visionConeMesh;
    }
}