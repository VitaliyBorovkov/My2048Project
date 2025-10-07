using UnityEngine;

public static class AimingService
{
    private const float maxRay = 1000f;

    public static Vector3 GetAimWorldPoint(Camera camera, Vector3 pointerScreenPosition,
        Transform spawnPoint, LayerMask aimMask)
    {
        if (camera == null || spawnPoint == null)
        {
            return spawnPoint != null ? spawnPoint.position : Vector3.zero;
        }

        Ray ray = camera.ScreenPointToRay(pointerScreenPosition);
        if (Physics.Raycast(ray, out var hit, maxRay, aimMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        Plane plane = new Plane(-camera.transform.forward, spawnPoint.position);
        return plane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : spawnPoint.position;
    }

    public static Vector3 ClampPull(Vector3 target, Vector3 spawnPosition, float maxPullDistance)
    {
        Vector3 direction = target - spawnPosition;
        float sqare = maxPullDistance * maxPullDistance;
        return direction.sqrMagnitude <= sqare ? target : spawnPosition + direction.normalized * maxPullDistance;
    }
}