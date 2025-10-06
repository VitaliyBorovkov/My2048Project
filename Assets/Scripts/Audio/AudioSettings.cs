using UnityEngine;

[CreateAssetMenu(menuName = "Game/Audio/AudioSettings", fileName = "AudioSettings")]
public class AudioSettings : ScriptableObject
{
    [Header("Music")]
    public AudioClip backgroundMusic;

    public bool musicLoop = true;
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Header("SFX")]
    [Tooltip("Clip to play when cubes merge.")]

    public AudioClip cubeMergeSound;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    public bool HasMergeClip => cubeMergeSound != null;
}