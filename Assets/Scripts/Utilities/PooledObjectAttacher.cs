using UnityEngine;

public static class PooledObjectAttacher
{
    public static void Attach(GameObject cube, CubePool owner)
    {
        if (cube == null)
        {
            return;
        }

        var pooledObject = cube.GetComponent<PooledObject>();
        if (pooledObject == null)
        {
            pooledObject = cube.AddComponent<PooledObject>();
        }

        if (owner != null)
        {
            pooledObject.OwnerPool = owner;
        }
    }
}
