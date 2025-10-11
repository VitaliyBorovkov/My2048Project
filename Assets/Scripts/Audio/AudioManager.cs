using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string LOG = "AudioManager";

    [SerializeField] private AudioSettings audioSettings;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float runtimeMusicVolume;
    private float runtimeSfxVolume;

    private void Awake()
    {
        var existing = FindObjectsByType<AudioManager>(FindObjectsSortMode.None);
        foreach (var item in existing)
        {
            if (item != this)
            {
                Debug.Log($"{LOG}: Another AudioManager exists. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
        }

        DontDestroyOnLoad(gameObject);

        if (audioSettings == null)
        {
            Debug.LogError($"{LOG}: audioSettings is not assigned in inspector.");
            return;
        }

        runtimeMusicVolume = PlayerPrefs.HasKey(PlayerPrefsKeys.MusicVolumeKey)
          ? PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolumeKey)
          : 1f;

        runtimeSfxVolume = PlayerPrefs.HasKey(PlayerPrefsKeys.SfxVolumeKey)
            ? PlayerPrefs.GetFloat(PlayerPrefsKeys.SfxVolumeKey)
            : 1f;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = Mathf.Clamp01(audioSettings.musicVolume);
        musicSource.loop = audioSettings.musicLoop;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.loop = false;
        sfxSource.volume = Mathf.Clamp01(audioSettings.sfxVolume);

        PlayMusic();
    }

    public void PlayMusic()
    {
        if (audioSettings == null)
        {
            Debug.LogError($"{LOG}: PlayMusic failed - audioSettings == null.");
            return;
        }

        if (audioSettings.backgroundMusic == null)
        {
            Debug.LogError($"{LOG}: PlayMusic failed - backgroundMusic clip == null.");
            return;
        }

        if (musicSource.clip == audioSettings.backgroundMusic && musicSource.isPlaying)
        {
            Debug.Log($"{LOG}: music already playing.");
            return;
        }

        musicSource.clip = audioSettings.backgroundMusic;
        musicSource.loop = audioSettings.musicLoop;
        musicSource.volume = Mathf.Clamp01(audioSettings.musicVolume);
        musicSource.Play();
    }

    public void PlayMergeSound()
    {
        if (audioSettings == null)
        {
            Debug.LogError($"{LOG}: PlayMergeSound failed - audioSettings == null.");
            return;
        }

        var clip = audioSettings.cubeMergeSound;
        if (clip == null)
        {
            Debug.LogWarning($"{LOG}: PlayMergeSound: cubeMergeClip is null.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning($"{LOG}: PlayMergeSound: sfxSource is null.");
            return;
        }

        sfxSource.volume = Mathf.Clamp01(audioSettings.sfxVolume);
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float value)
    {
        runtimeMusicVolume = Mathf.Clamp01(value);
        if (musicSource != null)
        {
            musicSource.volume = runtimeMusicVolume;
        }

        PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolumeKey, runtimeMusicVolume);
    }

    public float GetMusicVolume()
    {
        return runtimeMusicVolume;
    }

    public void SetSfxVolume(float value)
    {
        runtimeSfxVolume = Mathf.Clamp01(value);
        if (sfxSource != null)
        {
            sfxSource.volume = runtimeSfxVolume;
        }

        PlayerPrefs.SetFloat(PlayerPrefsKeys.SfxVolumeKey, runtimeSfxVolume);
    }

    public float GetSfxVolume() => runtimeSfxVolume;

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}