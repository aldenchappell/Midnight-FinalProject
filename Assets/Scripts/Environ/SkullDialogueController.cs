using UnityEngine;

public class SkullDialogueController : MonoBehaviour
{
    private AudioSource _audioSource;
    public NPCInteractionEvent npcInteractionEvent; 

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void HandleSkullCompanionSpeak(string[] dialogueLines, AudioClip clip)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogError("Dialogue lines are null or empty!");
            return;
        }

        DialogueController.Instance.StartDialogue(dialogueLines);

        if (clip != null)
        {
            Speak(clip);
        }
    }

    public void Speak(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    // Wrapper method for StartDialogue
    public void StartDialogue(string[] lines)
    {
        DialogueController.Instance.StartDialogue(lines);
    }

    // Wrapper method for StartDialogueWithAudio
    public void StartDialogueWithAudio(string[] lines, AudioClip[] audioClips)
    {
        DialogueController.Instance.StartDialogueWithAudio(lines, audioClips);
    }
}