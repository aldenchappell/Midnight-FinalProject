using UnityEngine;

public interface IPlaySkullDialogue
{
    void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip);
    void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clip);
    void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip);
}
