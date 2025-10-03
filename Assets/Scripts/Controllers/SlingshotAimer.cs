using UnityEngine;

public sealed class SlingshotAimer : MonoBehaviour
{
    private const string LOG = "SlingshotAimer";

    [SerializeField] private SlingshotView slingshotView;
    [SerializeField] private float maxPullDistance = 3f;
    [SerializeField] private LayerMask aimMask;

    private Camera mainCamera;
    private Transform spawnPoint;

    private void Awake()
    {
        if (slingshotView == null) Debug.LogWarning($"{LOG}: SlingshotView not assigned.");
        mainCamera = slingshotView != null ? slingshotView.MainCamera : Camera.main;
        spawnPoint = slingshotView != null ? slingshotView.SpawnPoint : null;
    }

    public void UpdateAiming(Vector3 screenPosition, GameObject currentCube)
    {
        if (slingshotView == null || mainCamera == null || spawnPoint == null || currentCube == null)
        {
            return;
        }

        Vector3 aimPoint = AimingService.GetAimWorldPoint(mainCamera, screenPosition, spawnPoint, aimMask);
        Vector3 clamped = AimingService.ClampPull(aimPoint, spawnPoint.position, maxPullDistance);

        slingshotView.SetCubePosition(currentCube, clamped);
        slingshotView.DrawLine(spawnPoint.position, clamped);
    }

    public Vector3 GetSpawnPointPosition()
    {
        return spawnPoint != null ? spawnPoint.position : Vector3.zero;
    }

    public Transform GetSpawnPointTransform()
    {
        return spawnPoint;
    }
}
