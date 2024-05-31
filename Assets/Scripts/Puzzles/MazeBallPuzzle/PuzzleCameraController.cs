using UnityEngine;
using Cinemachine;

public class PuzzleCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera puzzleCam;
    public GameObject mazePuzzleObj;

    [SerializeField] private Vector3 offset = new Vector3(0, 10, 0);

    private void LateUpdate()
    {
        if (puzzleCam != null && mazePuzzleObj != null)
        {
            // Position the camera above the mazePuzzleObj
            puzzleCam.transform.position = mazePuzzleObj.transform.position + offset;

            // Make the camera look at the mazePuzzleObj
            puzzleCam.transform.LookAt(mazePuzzleObj.transform);
        }
    }
}