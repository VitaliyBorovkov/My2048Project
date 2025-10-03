using UnityEngine;

[System.Serializable]
public struct LevelMergeRule
{
    public int level;

    [Min(2)]
    public int requiredCount;

    public float mergeRadius;

    public Color resultColor;
}