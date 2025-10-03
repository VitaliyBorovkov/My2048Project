using UnityEngine;

public sealed class SlingshotLauncher : MonoBehaviour
{
    private const string LOG = "SlingshotLauncher";

    [SerializeField] private float maxPullDistance = 3f;
    [SerializeField] private float maxForce = 25f;

    public bool Launch(Rigidbody targetRigidbody, Vector3 spawnPointPosition, Vector3 cubePosition)
    {
        if (targetRigidbody == null)
        {
            Debug.LogWarning($"{LOG}: targetRigidbody is null.");
            return false;
        }

        Vector3 pull = spawnPointPosition - cubePosition;
        float pullDistance = Mathf.Clamp(pull.magnitude, 0f, maxPullDistance);

        if (pullDistance <= 0.01f)
        {
            targetRigidbody.isKinematic = false;
            Debug.Log($"{LOG}: Released with no pull.");
            return true;
        }

        Vector3 direction = pull.normalized;
        float force = (pullDistance / maxPullDistance) * maxForce;

        targetRigidbody.isKinematic = false;
        targetRigidbody.AddForce(direction * force, ForceMode.VelocityChange);

        return true;
    }
}