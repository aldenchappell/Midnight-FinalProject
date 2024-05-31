using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBlockPuzzlePiece : MonoBehaviour
{
    public int puzzlePieceID;
    public Vector3 GetPosition
    {
        get
        {
            return GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
