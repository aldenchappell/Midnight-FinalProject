using UnityEngine;

public class OnlyDialogueNPC : DialogueNPCBase
{
    public SO_NpcDialogue dialogue;

    public override void Interact()
    {
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.StartDialogue(dialogue.dialogueLines);
        }
        else
        {
            Debug.LogWarning("DialogueController instance not found!");
        }
    }
}