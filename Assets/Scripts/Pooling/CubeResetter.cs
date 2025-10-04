using UnityEngine;

public static class CubeResetter
{
    public static void ResetState(GameObject cube)
    {
        if (cube == null)
        {
            return;
        }

        var mergeHandler = cube.GetComponent<CubeMergeHandler>();
        if (mergeHandler != null)
        {
            mergeHandler.ResetMergeState();
        }

        var cubeLevel = cube.GetComponent<CubeLevel>();
        if (cubeLevel != null)
        {
            cubeLevel.ResetVisualToDefault();
        }
    }
}
