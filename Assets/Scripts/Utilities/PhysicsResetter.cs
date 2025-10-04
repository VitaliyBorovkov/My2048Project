using UnityEngine;

public static class PhysicsResetter
{
    public static void ResetVelocity(GameObject cube)
    {
        if (cube == null)
        {
            return;
        }

        var rigidbody = cube.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            return;
        }

        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
