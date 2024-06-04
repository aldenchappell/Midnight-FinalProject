public class OnlyDialogueNPC : DialogueNPCBase
{
    public SO_NpcDialogue dialogue;

    public override void Interact()
    {
        DialogueController.Instance.StartDialogue(dialogue.dialogueLines);
    }
}