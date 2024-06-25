using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialoguePersonInfo", menuName = "ScriptableObjects/DialoguePersonInfo", order = 1)]
public class DialogueAudioPersonalitySO : ScriptableObject
{
    [Space(3)]
    public string id;

    [Space(5)]
    [Header("Audio Clips")]
    public AudioClip[] dialogue_clips;

    [Space(5)]
    [Header("Personality")]
    // frequency of sound
    [Range(1, 5)]
    public int dialogue_frequencyLevel;
    // minimum pitch
    [Range(-3, 3)]
    public float dialogue_minPitch;
    // maximum pitch
    [Range(-3, 3)]
    public float dialogue_maxPitch;
    // stop sound to play next
    public bool stopSound;
    [Range(0f, 1f)]
    public float dialogue_volume;
}
