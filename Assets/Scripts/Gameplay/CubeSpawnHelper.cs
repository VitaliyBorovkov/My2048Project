using UnityEngine;

public static class CubeSpawnHelper
{
    public static void ApplyInitialLevel(GameObject cube)
    {
        if (cube == null)
        {
            return;
        }

        var cubeLevel = cube.GetComponent<CubeLevel>();
        if (cubeLevel == null)
        {
            return;
        }

        float randomValue = Random.value;
        if (randomValue < 0.75f)
        {
            cubeLevel.SetLevel(1);
        }
        else
        {
            cubeLevel.SetLevel(2);
        }
    }
}
