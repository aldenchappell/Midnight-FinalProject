using System.Collections;
using UnityEngine;

public interface IPlaySkullDialogue
{
    void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip);
    void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips);
    void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip);
    IEnumerator PlaySkullDialoguePuzzleHintClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip);
}
