using UnityEngine;

public sealed class SlingshotLauncher : MonoBehaviour
{
    private const string LOG = "SlingshotLauncher";

    [SerializeField] private float maxPullDistance = 3f;
    [SerializeField] private float maxForce = 25f;
    [SerializeField] private float minLaunchDistance = 0.2f;

    public bool Launch(Rigidbody targetRigidbody, Vector3 spawnPointPosition, Vector3 cubePosition)
    {
        if (targetRigidbody == null)
        {
            Debug.LogWarning($"{LOG}: targetRigidbody is null.");
            return false;
        }

        Vector3 pull = spawnPointPosition - cubePosition;
        float pullDistance = Mathf.Clamp(pull.magnitude, 0f, maxPullDistance);

        if (pullDistance < minLaunchDistance)
        {
            Debug.Log($"{LOG}: Launch aborted - pullDistance {pullDistance:F2} < minLaunchDistance {minLaunchDistance:F2}. Restoring cube to spawn point.");

            targetRigidbody.isKinematic = true;

            PhysicsResetter.ResetVelocity(targetRigidbody.gameObject);
            CubeResetter.ResetState(targetRigidbody.gameObject);

            targetRigidbody.transform.position = spawnPointPosition;

            return false;
        }

        PhysicsResetter.ResetVelocity(targetRigidbody.gameObject);

        targetRigidbody.isKinematic = false;
        targetRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        Vector3 direction = pull.normalized;
        float force = (pullDistance / maxPullDistance) * maxForce;

        targetRigidbody.AddForce(direction * force, ForceMode.VelocityChange);

        return true;
    }
}