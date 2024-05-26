using UnityEngine;

[CreateAssetMenu(menuName = "SO_GroundType/Ground Type", order = 0)]
public class SO_GroundType : ScriptableObject
{
    //Assign a reference to what sound should be played when the player is on this specific ground type.
    //i.e when the player is on wood, or carpet, etc..
    public AudioClip[] groundTypeAudioClips;

    public float groundTypeVolumeValue; 
    
    public string groundTypeName;
}
