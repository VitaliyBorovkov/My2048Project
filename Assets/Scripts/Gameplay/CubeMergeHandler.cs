using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public sealed class CubeMergeHandler : MonoBehaviour
{
    private const string LOG = "CubeMergeHandler";

    [Header("Settings")]
    [SerializeField] private CubeMergeSettings mergeSettings;
    [SerializeField] private float mergeSearchRadiusFallback = 0.6f;
    [SerializeField] private float mergeCooldown = 0.12f;
    [SerializeField] private LayerMask mergeMask;

    private CubeLevel cubeLevel;
    private Rigidbody rigid;
    private float lastMergeTime;
    private bool isMerging;

    private void Awake()
    {
        cubeLevel = GetComponent<CubeLevel>();
        rigid = GetComponent<Rigidbody>();

        if (cubeLevel == null)
        {
            Debug.LogWarning($"{LOG}: CubeLevel not found on the object.");
        }

        if (rigid == null)
        {
            Debug.LogWarning($"{LOG}: Rigidbody not found on the object.");
        }

        if (mergeSettings == null)
        {
            Debug.LogWarning($"{LOG}: Merge settings asset is not assigned on {gameObject.name}.");
        }

    }

    public void ResetMergeState()
    {
        isMerging = false;
        lastMergeTime = -999f;
    }
    private bool CanMergeNow()
    {
        return !isMerging && (Time.time - lastMergeTime) >= mergeCooldown;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMerging || cubeLevel == null || !CanMergeNow())
        {
            return;
        }

        CubeMergeHandler other = collision.collider.GetComponentInParent<CubeMergeHandler>();
        if (other != null && other != this && other.cubeLevel != null &&
            other.cubeLevel.Level == cubeLevel.Level)
        {
            LevelMergeRule directRule = GetRuleForCurrentLevel();
            if (directRule.requiredCount == 2)
            {
                if (GetInstanceID() >= other.GetInstanceID())
                {
                    TryPairMerge(other, collision, directRule);
                }
                return;
            }
        }

        LevelMergeRule levelMergeRule = GetRuleForCurrentLevel();
        float searchRadius = levelMergeRule.mergeRadius > 0f ? levelMergeRule.mergeRadius : mergeSearchRadiusFallback;

        Vector3 origin = (collision.contacts != null && collision.contacts.Length > 0) ?
            collision.contacts[0].point : transform.position;

        Collider[] overlaps = Physics.OverlapSphere(origin, searchRadius, mergeMask, QueryTriggerInteraction.Ignore);

        var candidates = new List<CubeMergeHandler>(levelMergeRule.requiredCount);
        for (int i = 0; i < overlaps.Length; i++)
        {
            Collider collider = overlaps[i];
            if (collider == null)
            {
                continue;
            }

            CubeMergeHandler cubeMergeHandler = collider.GetComponentInParent<CubeMergeHandler>();
            if (cubeMergeHandler == null || cubeMergeHandler.cubeLevel == null)
            {
                continue;
            }

            if (cubeMergeHandler.cubeLevel.Level != cubeLevel.Level)
            {
                continue;
            }
            if (!cubeMergeHandler.CanMergeNow())
            {
                continue;
            }
            if (candidates.Contains(cubeMergeHandler))
            {
                continue;
            }

            candidates.Add(cubeMergeHandler);
            if (candidates.Count >= levelMergeRule.requiredCount)
            {
                break;
            }
        }

        if (candidates.Count < levelMergeRule.requiredCount)
        {
            return;
        }

        int maxId = int.MinValue;
        foreach (var cube in candidates)
        {
            int id = cube.GetInstanceID();
            if (id > maxId) maxId = id;
        }

        if (GetInstanceID() != maxId)
        {
            return;
        }

        PerformGroupMerge(candidates, levelMergeRule);
    }

    private void TryPairMerge(CubeMergeHandler other, Collision collision, LevelMergeRule rule)
    {
        if (other == null || other == this || other.cubeLevel == null || cubeLevel == null)
        {
            return;
        }

        if (!other.CanMergeNow() || !CanMergeNow())
        {
            return;
        }

        if (GetInstanceID() < other.GetInstanceID())
        {
            return;
        }

        Vector3 contactPoint = (collision.contacts != null && collision.contacts.Length > 0) ?
            collision.contacts[0].point : (transform.position + other.transform.position) * 0.5f;

        var group = new List<CubeMergeHandler>(2) { this, other };
        PerformGroupMerge(group, rule);

        transform.position = contactPoint;
    }

    private void PerformGroupMerge(List<CubeMergeHandler> cubeMerge, LevelMergeRule levelMergeRule)
    {
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


        if (!cubeMerge.Contains(this))
        {
            Debug.LogWarning($"{LOG}: Initiator not in group, aborting merge.");
            return;
        }

        foreach (var cube in cubeMerge)
        {
            cube.isMerging = true;
        }

        Vector3 avgPosition = Vector3.zero;
        Vector3 avgVelocity = Vector3.zero;
        float totalMass = 0f;

        for (int i = 0; i < cubeMerge.Count; i++)
        {
            var cube = cubeMerge[i];
            avgPosition += cube.transform.position;
            if (cube.rigid != null)
            {
                avgVelocity += cube.rigid.linearVelocity;
            }
            totalMass += (cube.rigid != null ? cube.rigid.mass : 1f);
        }

        avgPosition /= cubeMerge.Count;
        avgVelocity /= cubeMerge.Count;

        survivor.transform.position = avgPosition;

        var survivorLevel = survivor.cubeLevel;
        if (survivorLevel != null)
        {
            survivorLevel.IncreaseLevel(1);
        }

        Rigidbody survivorRb = survivor.rigid;
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

        survivor.lastMergeTime = Time.time;
        lastMergeTime = Time.time;

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

        StartCoroutine(ResetMergingCoroutine(survivor));
    }

    private IEnumerator ResetMergingCoroutine(CubeMergeHandler handler)
    {
        if (handler == null)
            yield break;

        yield return new WaitForSeconds(Mathf.Max(0.01f, mergeCooldown));

        handler.isMerging = false;
        handler.lastMergeTime = Time.time;
    }

    private LevelMergeRule GetRuleForCurrentLevel()
    {
        if (mergeSettings != null)
        {
            return mergeSettings.GetRuleForLevel(cubeLevel.Level);
        }

        return new LevelMergeRule
        {
            level = cubeLevel.Level,
            requiredCount = 2,
            mergeRadius = mergeSearchRadiusFallback,
            resultColor = Color.white
        };
    }
}