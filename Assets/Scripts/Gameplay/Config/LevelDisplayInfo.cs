using System;

using UnityEngine;

[Serializable]
public struct LevelDisplayInfo
{
    [Min(1)] public int level;
    public string displayText;
    [Min(1f)] public float fontSize;
}