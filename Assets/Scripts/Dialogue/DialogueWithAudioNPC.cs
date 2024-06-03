using UnityEngine;

public class DialogueWithAudioNPC : DialogueNPCBase
{
    public SO_NPCDialogueWithAudio dialogue;

    public override void Interact()
    {
        DialogueController.Instance.StartDialogueWithAudio(dialogue.dialogueLines, dialogue.audioClips);
    }
}