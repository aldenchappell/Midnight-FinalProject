using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NPCInteractionEvent : UnityEvent<NpcDialogue> { }

public class InteractableNPC : InteractableObject
{
    public NPCInteractionEvent onNPCInteraction;
    private NpcDialogue _npcDialogue;

    private void Awake()
    {
        _npcDialogue = GetComponent<NpcDialogue>();
        if (_npcDialogue != null)
        {
            onNPCInteraction.AddListener(TriggerDialogue);
        }
    }

    public void TriggerDialogue(NpcDialogue npcDialogue)
    {
        DialogueController.Instance.StartDialogue(npcDialogue.SO_NpcDialogue.dialogueLines);
    }

    public void Interact()
    {
        if (_npcDialogue != null)
        {
            onNPCInteraction.Invoke(_npcDialogue);
            DialogueController.Instance.EnableDialogueBox();
        }
    }
}