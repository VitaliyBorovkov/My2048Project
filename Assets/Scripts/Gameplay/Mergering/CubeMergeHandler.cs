using System.Collections.Generic;

using UnityEngine;

public sealed class CubeMergeHandler : MonoBehaviour
{
    private const string LOG = "CubeMergeHandler";

    [Header("Settings")]
    [SerializeField] private float mergeSearchRadiusFallback = 0.6f;
    [SerializeField] private float mergeCooldown = 0.12f;
    [SerializeField] private LayerMask mergeMask;

    [Header("References")]
    [SerializeField] private CubeMergeSettings mergeSettings;
    [SerializeField] private CubeLevel cubeLevel;

    private ScoreService scoreService;
    private Rigidbody rigid;
    private float lastMergeTime;
    private bool isMerging;

    private void Awake()
    {
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

    public void SetScoreService(ScoreService scoreService)
    {
        this.scoreService = scoreService;
    }

    public Rigidbody Rigid => rigid;
    public CubeLevel CubeLevel => cubeLevel;

    public void BeginMerging()
    {
        isMerging = true;
    }

    public void EndMerging()
    {
        isMerging = false;
    }

    public void SetLastMergeTime(float time)
    {
        lastMergeTime = time;
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
        float searchRadius = levelMergeRule.mergeRadius > 0f ? levelMergeRule.mergeRadius :
            mergeSearchRadiusFallback;

        Vector3 origin = (collision.contacts != null && collision.contacts.Length > 0) ?
            collision.contacts[0].point : transform.position;

        var candidates = CollectCandidates(origin, searchRadius, levelMergeRule);

        if (candidates.Count < levelMergeRule.requiredCount)
        {
            return;
        }

        int maxId = int.MinValue;
        foreach (var cube in candidates)
        {
            int id = cube.GetInstanceID();
            if (id > maxId)
            {
                maxId = id;
            }
        }

        if (GetInstanceID() != maxId)
        {
            return;
        }

        CubeMergeProcessor.PerformGroupMerge(this, candidates, levelMergeRule, mergeSettings,
            mergeCooldown, scoreService);
    }

    private List<CubeMergeHandler> CollectCandidates(Vector3 origin, float searchRadius, LevelMergeRule levelMergeRule)
    {
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

        return candidates;
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
        CubeMergeProcessor.PerformGroupMerge(this, group, rule, mergeSettings, mergeCooldown, scoreService);

        transform.position = contactPoint;
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