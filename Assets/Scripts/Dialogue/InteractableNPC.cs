using UnityEngine.Events;

[System.Serializable]
public class NPCInteractionEvent : UnityEvent<string[]> { }

public class InteractableNPC : InteractableObject
{
    public NPCInteractionEvent onNPCInteraction;
    private NpcDialogue _npcDialogue;

    private void Awake()
    {
        _npcDialogue = GetComponent<NpcDialogue>();
        if (_npcDialogue != null)
        {
            onNPCInteraction.AddListener(DialogueController.Instance.StartDialogue);
        }
    }

    public void Interact()
    {
        if (_npcDialogue != null)
        {
            onNPCInteraction.Invoke(_npcDialogue.SO_NpcDialogue.dialogueLines);
        }
    }
}