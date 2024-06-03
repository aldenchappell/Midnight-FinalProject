using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC_DialogueWithAudio", order = 1)]
public class SO_NPCDialogueWithAudio : ScriptableObject
{
    public string[] dialogueLines;
    public AudioClip[] audioClips;
}
