using System.Collections;
using UnityEngine;

public interface IPlaySkullDialogue
{
    /// <summary>
    /// MAKE SURE THAT IN EACH METHOD THAT IS INHERITED FROM THIS SCRIPT, THAT YOU USE
    /// if(!SkullDialogue.Instance.IsAudioSourcePlaying()) IN EACH METHOD
    /// </summary>
    /// <param name="source"></param>
    /// <param name="clip"></param>
    void PlaySpecificSkullDialogueClip(AudioSource source, AudioClip clip);
    /// <summary>
    /// MAKE SURE THAT IN EACH METHOD THAT IS INHERITED FROM THIS SCRIPT, THAT YOU USE
    /// if(!SkullDialogue.Instance.IsAudioSourcePlaying()) IN EACH METHOD
    /// </summary>
    /// <param name="source"></param>
    /// <param name="clip"></param>
    void PlayRandomSkullDialogueClip(AudioSource source, AudioClip[] clips);
    /// <summary>
    /// MAKE SURE THAT IN EACH METHOD THAT IS INHERITED FROM THIS SCRIPT, THAT YOU USE
    /// if(!SkullDialogue.Instance.IsAudioSourcePlaying()) IN EACH METHOD
    /// </summary>
    /// <param name="source"></param>
    /// <param name="clip"></param>
    void PlaySpecificSkullDialogueClipWithLogic(bool value, AudioSource source, AudioClip clip);
    /// <summary>
    /// MAKE SURE THAT IN EACH METHOD THAT IS INHERITED FROM THIS SCRIPT, THAT YOU USE
    /// if(!SkullDialogue.Instance.IsAudioSourcePlaying()) IN EACH METHOD
    /// </summary>
    /// <param name="source"></param>
    /// <param name="clip"></param>
    IEnumerator PlaySkullDialoguePuzzleHintClip(int indexOfCurrentLevelPuzzles, AudioSource source, AudioClip clip);
}
