using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsController : MonoBehaviour
{
    private const string LOG = "AudioOptionsController";

    [Header("References")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Settings")]
    [SerializeField] private bool useSingleSliderForAll = false;

    private float waitForManagerTimeout = 1f;

    private Coroutine initCoroutine;

    private void OnEnable()
    {
        if (audioManager != null && musicSlider != null && (sfxSlider != null || useSingleSliderForAll))
        {
            InitializeSlidersAndSubscribe();
        }
    }

    private void OnDisable()
    {
        UnsubscribeSliders();
        if (initCoroutine != null)
        {
            StopCoroutine(initCoroutine);
            initCoroutine = null;
        }
    }

    private void Start()
    {
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();

        if (audioManager != null)
        {
            InitializeSlidersAndSubscribe();
            return;
        }

        initCoroutine = StartCoroutine(WaitForAudioManagerAndInit(waitForManagerTimeout));
    }

    private IEnumerator WaitForAudioManagerAndInit(float timeout)
    {
        float timer = 0f;
        while (timer < timeout)
        {
            audioManager = FindAnyObjectByType<AudioManager>();
            if (audioManager != null) break;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (audioManager == null)
        {
            Debug.LogWarning($"{LOG}: AudioManager not found after waiting {timeout} sec. Sliders won't be initialized.");
            yield break;
        }

        InitializeSlidersAndSubscribe();
    }

    private void InitializeSlidersAndSubscribe()
    {
        if (musicSlider == null)
        {
            Debug.LogError($"{LOG}: musicSlider  are not assigned in inspector.");
            return;
        }

        if (!useSingleSliderForAll && sfxSlider == null)
        {
            Debug.LogWarning($"{LOG}: sfxSlider not assigned and useSingleSliderForAll is false. SFX will not be controllable.");
        }


        musicSlider.minValue = 0f;
        musicSlider.maxValue = 1f;

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
        }

        float musicVal = audioManager.GetMusicVolume();
        float sfxVal = audioManager.GetSfxVolume();

        if (useSingleSliderForAll)
        {
            musicSlider.SetValueWithoutNotify(musicVal);
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(musicVal);
            }

        }
        else
        {
            musicSlider.SetValueWithoutNotify(musicVal);
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(sfxVal);
            }

        }

        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        if (!useSingleSliderForAll && sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        }
    }

    private void UnsubscribeSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        if (audioManager == null)
        {
            Debug.LogWarning($"{LOG}: OnMusicSliderChanged called but audioManager is null.");
            return;
        }
        audioManager.SetMusicVolume(value);

        if (useSingleSliderForAll)
        {
            audioManager.SetSfxVolume(value);
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(value);
            }

        }
    }

    private void OnSfxSliderChanged(float value)
    {
        if (audioManager == null)
        {
            Debug.LogWarning($"{LOG}: OnSfxSliderChanged called but audioManager is null.");
            return;
        }
        audioManager.SetSfxVolume(value);
    }
}