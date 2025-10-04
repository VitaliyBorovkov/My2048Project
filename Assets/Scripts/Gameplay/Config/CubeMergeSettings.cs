using UnityEngine;

[CreateAssetMenu(fileName = "CubeMergeSettings", menuName = "Gameplay/Cube Merge Settings", order = 100)]
public sealed class CubeMergeSettings : ScriptableObject
{
    [Header("Defaults")]
    [Min(2)] public int defaultRequiredCount = 2;
    [Min(0.01f)] public float defaultMergeRadius = 0.6f;
    public Color defaultResultColor = Color.white;

    [Header("Per-level rules")]
    public LevelMergeRule[] rules = new LevelMergeRule[0];

    public LevelMergeRule GetRuleForLevel(int level)
    {
        if (rules != null)
        {
            for (int i = 0; i < rules.Length; i++)
            {
                if (rules[i].level == level)
                {
                    LevelMergeRule levelMergeRule = rules[i];
                    if (levelMergeRule.requiredCount < 2)
                    {
                        levelMergeRule.requiredCount = defaultRequiredCount;
                    }

                    if (levelMergeRule.mergeRadius <= 0f)
                    {
                        levelMergeRule.mergeRadius = defaultMergeRadius;
                    }

                    if (levelMergeRule.resultColor == default(Color))
                    {
                        levelMergeRule.resultColor = defaultResultColor;
                    }
                    return levelMergeRule;
                }
            }
        }

        LevelMergeRule def = new LevelMergeRule
        {
            level = level,
            requiredCount = defaultRequiredCount,
            mergeRadius = defaultMergeRadius,
            resultColor = defaultResultColor
        };

        return def;
    }
}