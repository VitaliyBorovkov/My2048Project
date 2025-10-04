using UnityEngine;

[CreateAssetMenu(fileName = "CubeValueConfig", menuName = "Gameplay/Cube Value Config", order = 120)]
public sealed class CubeValueConfig : ScriptableObject
{
    private const string LOG = "CubeValueConfig";

    public bool useAutoPow2 = true;
    public float defaultFontSize = 10f;
    public LevelDisplayInfo[] perLevel = new LevelDisplayInfo[0];

    public string GetDisplayText(int level)
    {
        if (perLevel != null)
        {
            for (int i = 0; i < perLevel.Length; i++)
            {
                if (perLevel[i].level == level && !string.IsNullOrEmpty(perLevel[i].displayText))
                {
                    return perLevel[i].displayText;
                }
            }
        }

        if (useAutoPow2)
        {
            int value = 1 << level;
            return value.ToString();
        }

        return string.Empty;
    }
    public float GetFontSize(int level)
    {
        if (perLevel != null)
        {
            for (int i = 0; i < perLevel.Length; i++)
            {
                if (perLevel[i].level == level && perLevel[i].fontSize > 0f)
                {
                    return perLevel[i].fontSize;
                }
            }
        }

        return defaultFontSize;
    }
}
