using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CompassMarker", order = 4)]
public class SO_CompassMarker : ScriptableObject
{
    public Sprite compassMarkerSprite;
    public Vector2 compassMarkerSpriteSize = new Vector2(100,100);
}
