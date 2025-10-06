using UnityEngine;

public class CubeMergeSoundInvoker : MonoBehaviour
{
    private const string LOG = "CubeMergeSoundInvoker";

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogWarning($"{LOG}: AudioManager not found. Merge SFX won't play until AudioManager exists.");
        }
    }

    private void OnEnable()
    {
        CubeMergeProcessor.MergeCompletedDetailed += OnMergeCompletedDetailed;
    }

    private void OnDisable()
    {
        CubeMergeProcessor.MergeCompletedDetailed -= OnMergeCompletedDetailed;
    }

    private void OnMergeCompletedDetailed(GameObject survivor, int resultLevel, int awardedPoints, int[] sourceLevels)
    {
        if (audioManager != null)
        {
            audioManager.PlayMergeSound();
        }
    }
}