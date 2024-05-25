Implementing a new GroundType for footsteps.

First, on the ground/object that is different from tile, add the GroundType component to the object.

In the inspector aftering adding the component, there will be a slot for an SO_GroundType scriptable object.

To find these, go into the ScriptableObjects folder->GroundTypes

To make a new ground type, right click in the folder, go up to SO_GroundType->GroundType 

Next, once a bunch of sound effects for the new ground type have been imported,
 drag them into the array in the inspector for the newly created ground type scriptable object.
 
Finally, ensure that the ground object is on the ground layer,
 and assign the newly created scriptable object to the GroundType component in the inspector.