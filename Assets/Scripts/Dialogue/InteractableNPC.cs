using UnityEngine;

public class InteractableNPC : InteractableObject
{
    public NPCInteractionEvent onNPCInteraction;
    private NpcDialogue _npcDialogue;
    private DialogueWithAudioNPC _npc;

    private void Awake()
    {
        _npcDialogue = GetComponent<NpcDialogue>();
    }

    public void Interact()
    {
        if (_npcDialogue != null)
        {
            if (TryGetComponent<SkullDialogueController>(out SkullDialogueController skullController))
            {
                skullController.HandleSkullCompanionSpeak(_npcDialogue.SO_NpcDialogue.dialogueLines, _npc.dialogue.audioClips[0]);
            }
            else
            {
                onNPCInteraction.Invoke(_npcDialogue.SO_NpcDialogue.dialogueLines, null);
            }
        }
    }
}