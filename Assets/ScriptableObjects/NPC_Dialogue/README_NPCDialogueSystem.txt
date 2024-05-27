NPC Dialogue System Implementation

Step 1. Create a new SO_NpcDialogue object.

To do this, go into the ScriptableObjects folder->NPC_Dialogue->Right click->Create->Dialogue-NPC_Dialogue.
To edit this NPC's lines, simply go to the newly created scriptable object and add to the lines array. 
The lines should be in the order in which you wish them to be read to the player in.

Step 2. Add required components to the NPC.

On the NPC game object, add them to the InteractableObject layer mask (The NPC should have some sort of collider on the parent object).
Next, add the NPCDialogue component to the NPC. 
Assign the newly created/existing NPC_Dialogue scriptable object in the inspector.


Step 3. Hook up functionality for the NPC.

Next, add the InteractableNPC component to the NPC. 
For the OnInteraction() event, assign the NPC object for the source of the event, and use InteractableNPC->Interact for the method.
Assign the TempDialogueBubble sprite as the interaction icon, and use 50x50 for the icon size.
For the OnNPCInteraction() event, drag in the DialogueController from the player for the source of the event, and use DialogueController->StartDialogue as the method.

Done! Ask Alden if you have any questions! :)

