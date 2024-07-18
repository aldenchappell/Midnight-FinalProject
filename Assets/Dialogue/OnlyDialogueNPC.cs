using UnityEngine;

public class OnlyDialogueNPC : DialogueNPCBase
{
    public SO_NpcDialogue dialogue;

    private PlayerDualHandInventory _inventory;
    private bool _canInteract;

    private void Awake()
    {
        _inventory = GameObject.FindAnyObjectByType<PlayerDualHandInventory>();
        _canInteract = true;
    }


    public override void Interact()
    {
        if(_canInteract)
        {
            if (DialogueController.Instance != null)
            {
                print("Interacting");
                DialogueController.Instance.StartDialogue(dialogue.dialogueLines, this.gameObject);
                _canInteract = false;
                Invoke("ChangeInteract", 4f);
            }
            else
            {
                Debug.LogWarning("DialogueController instance not found!");
            }
        }
    }

    private void ChangeInteract()
    {
        _canInteract = true;
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