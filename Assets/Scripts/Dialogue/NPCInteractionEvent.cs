using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class NPCInteractionEvent : UnityEvent<string[], AudioClip>
{
    public NPCInteractionEvent OnSkullCompanionSpeak;
}