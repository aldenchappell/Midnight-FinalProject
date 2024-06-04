public class InteractableNPC : InteractableObject
{
    public NPCInteractionEvent onNPCInteraction;
    private NpcDialogue _npcDialogue;

    private void Awake()
    {
        _npcDialogue = GetComponent<NpcDialogue>();
    }

    public void Interact()
    {
        if (_npcDialogue != null)
        {
            onNPCInteraction.Invoke(_npcDialogue.SO_NpcDialogue.dialogueLines, null);
        }
    }
}