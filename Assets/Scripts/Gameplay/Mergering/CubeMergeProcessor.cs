using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class CubeMergeProcessor
{
    private const string LOG = "CubeMergeProcessor";

    //public static event Action<GameObject, int> MergeCompleted;

    public static event Action<GameObject, int, int, int[]> MergeCompletedDetailed;

    public static void PerformGroupMerge(MonoBehaviour owner, List<CubeMergeHandler> cubeMerge,
        LevelMergeRule levelMergeRule, CubeMergeSettings mergeSettings, float mergeCooldown, ScoreService scoreService)
    {
        if (owner == null)
        {
            Debug.LogWarning($"{LOG}: owner is null, cannot start merge coroutine.");
            return;
        }

        if (cubeMerge == null || cubeMerge.Count == 0)
        {
            Debug.LogWarning($"{LOG}: Empty merge group.");
            return;
        }

        CubeMergeHandler survivor = null;
        int maxId = int.MinValue;
        foreach (var cube in cubeMerge)
        {
            if (cube == null)
            {
                continue;
            }
            int id = cube.GetInstanceID();
            if (id > maxId)
            {
                maxId = id;
                survivor = cube;
            }
        }

        if (survivor == null)
        {
            Debug.LogWarning($"{LOG}: No survivor found, aborting merge.");
            return;
        }

        if (!cubeMerge.Contains(owner.GetComponent<CubeMergeHandler>()))
        {
            Debug.LogWarning($"{LOG}: Initiator not in group, aborting merge.");
            return;
        }

        foreach (var cube in cubeMerge)
        {
            if (cube == null)
            {
                continue;
            }
            cube.BeginMerging();
        }

        Vector3 avgPosition = Vector3.zero;
        Vector3 avgVelocity = Vector3.zero;
        float totalMass = 0f;

        for (int i = 0; i < cubeMerge.Count; i++)
        {
            var cube = cubeMerge[i];
            if (cube == null)
            {
                continue;
            }

            avgPosition += cube.transform.position;
            if (cube.Rigid != null)
            {
                avgVelocity += cube.Rigid.linearVelocity;
            }
            totalMass += (cube.Rigid != null ? cube.Rigid.mass : 1f);
        }

        avgPosition /= cubeMerge.Count;
        avgVelocity /= cubeMerge.Count;

        survivor.transform.position = avgPosition;

        var survivorLevel = survivor.CubeLevel;
        if (survivorLevel != null)
        {
            int[] sourceLevels = new int[cubeMerge.Count];
            for (int i = 0; i < cubeMerge.Count; i++)
            {
                var c = cubeMerge[i];
                sourceLevels[i] = (c != null && c.CubeLevel != null) ? c.CubeLevel.Level : -1;
            }

            survivorLevel.IncreaseLevel(1);

            int resultLevel = survivorLevel.Level;
            int mergedValue = 1 << resultLevel;

            int awardedPoints = 0;
            if (scoreService != null)
            {
                awardedPoints = scoreService.AddPointsForResultLevel(resultLevel);
            }

            //MergeCompleted?.Invoke(survivor.gameObject, survivorLevel.Level);
            MergeCompletedDetailed?.Invoke(survivor.gameObject, resultLevel, awardedPoints, sourceLevels);

            Rigidbody survivorRb = survivor.Rigid;
            if (survivorRb != null)
            {
                survivorRb.mass = totalMass;
                survivorRb.linearVelocity = avgVelocity;
                survivorRb.angularVelocity = Vector3.zero;
            }

            LevelMergeRule resultRule = levelMergeRule;
            if (mergeSettings != null && survivorLevel != null)
            {
                resultRule = mergeSettings.GetRuleForLevel(survivorLevel.Level);
            }

            if (survivorLevel != null)
            {
                survivorLevel.SetColor(resultRule.resultColor);
            }

            float now = Time.time;
            foreach (var cube in cubeMerge)
            {
                if (cube == null)
                {
                    continue;
                }
                cube.SetLastMergeTime(now);
            }

            for (int i = 0; i < cubeMerge.Count; i++)
            {
                var cube = cubeMerge[i];
                if (cube == null)
                {
                    continue;
                }
                if (cube == survivor)
                {
                    continue;
                }

                var marker = cube.GetComponent<PooledObject>();
                if (marker != null && marker.OwnerPool != null)
                {
                    marker.OwnerPool.Despawn(cube.gameObject);
                }
            }

            owner.StartCoroutine(ResetMergingCoroutine(survivor, mergeCooldown));
        }
    }

    public static IEnumerator ResetMergingCoroutine(CubeMergeHandler handler, float mergeCooldown)
    {
        if (handler == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(Mathf.Max(0.01f, mergeCooldown));

        handler.EndMerging();
        handler.SetLastMergeTime(Time.time);
    }
}
