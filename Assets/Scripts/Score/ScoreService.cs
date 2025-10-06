using System;

using UnityEngine;

public class ScoreService : MonoBehaviour
{
    private const string LOG = "ScoreService";

    [Header("Config")]
    [SerializeField] private ScoreConfig scoreConfig;

    public int TotalScore { get; private set; }

    public event Action<int> OnScoreChanged;

    public int AddPointsForResultLevel(int resultLevel)
    {
        if (resultLevel < 2)
        {
            Debug.LogWarning($"{LOG}: resultLevel={resultLevel} is invalid for merge scoring.");
            return 0;
        }

        int index = resultLevel - 2;
        int points = GetPointsByIndex(index);

        TotalScore += points;
        OnScoreChanged?.Invoke(TotalScore);

        return points;
    }

    public int AddByMergedValue(int mergedValue)
    {
        if (mergedValue < 4 || (mergedValue & (mergedValue - 1)) != 0)
        {
            Debug.LogWarning($"{LOG}: mergedValue={mergedValue} is invalid for merge scoring.");
            return 0;
        }

        int resultLevel = 0;
        int tmp = mergedValue;
        while (tmp > 1)
        {
            tmp >>= 1;
            resultLevel++;
        }
        int awarded = AddPointsForResultLevel(resultLevel);

        return awarded;
    }

    private int GetPointsByIndex(int index)
    {
        if (scoreConfig != null && scoreConfig.pointsByLevel != null &&
            index >= 0 && index < scoreConfig.pointsByLevel.Length)
        {
            return Mathf.Max(0, scoreConfig.pointsByLevel[index]);
        }

        return 1 << index;
    }
}
