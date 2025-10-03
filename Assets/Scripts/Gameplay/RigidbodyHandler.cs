using UnityEngine;

public sealed class RigidbodyHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody cachedRigidbody;

    private void Awake()
    {
        if (cachedRigidbody == null)
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        if (cachedRigidbody != null)
        {
            cachedRigidbody.linearVelocity = velocity;
        }
    }

    public Vector3 GetVelocity()
    {
        return cachedRigidbody != null ? cachedRigidbody.linearVelocity : Vector3.zero;
    }

    public void SetMass(float mass)
    {
        if (cachedRigidbody != null)
        {
            cachedRigidbody.mass = mass;
        }
    }

    public float GetMass()
    {
        return cachedRigidbody != null ? cachedRigidbody.mass : 1f;
    }
}