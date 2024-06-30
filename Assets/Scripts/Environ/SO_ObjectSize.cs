using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Object/ObjectSize", order = 6)]
public class SO_ObjectSize : ScriptableObject
{
    [Tooltip("Depending on the size of the object, this should be 0,1,2,3. 0 is extra small, 3 is big.")]
    [Range(0,4)] public int handPositionIndex = 0;
}
