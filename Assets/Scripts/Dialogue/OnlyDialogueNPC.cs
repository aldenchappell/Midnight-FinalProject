using UnityEngine;

public class OnlyDialogueNPC : DialogueNPCBase
{
    public SO_NpcDialogue dialogue;

    private PlayerDualHandInventory _inventory;

    private void Awake()
    {
        _inventory = GameObject.FindAnyObjectByType<PlayerDualHandInventory>();
    }


    public override void Interact()
    {
        if(IsSkullInInventory())
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

    private bool IsSkullInInventory()
    {
        /*
        foreach(GameObject item in _inventory.GetInventory)
        {
            if(item != null)
            {
                if (item.CompareTag("Skull"))
                {
                    return false;
                }
            }
        }
        */
        GameObject[] inventory = _inventory.GetInventory;
        if(inventory[_inventory.currentIndexSelected] != null)
        {
            if (inventory[_inventory.currentIndexSelected].CompareTag("Skull"))
            {
                return false;
            }
        }

        return true;
    }
}