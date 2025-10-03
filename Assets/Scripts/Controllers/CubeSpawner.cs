using UnityEngine;

public sealed class CubeSpawner : MonoBehaviour
{
    private const string LOG = "CubeSpawner";

    [Header("References")]
    [SerializeField] private CubePool cubePool;
    [SerializeField] private GameObject fallbackPrefab;

    public GameObject SpawnAt(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning($"{LOG}: spawnPoint is null.");
            return null;
        }

        GameObject cube;

        if (cubePool != null)
        {
            cube = cubePool.Spawn(spawnPoint.position, spawnPoint.rotation);
            if (cube == null && fallbackPrefab != null)
            {
                cube = Instantiate(fallbackPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
        else
        {
            if (fallbackPrefab == null)
            {
                Debug.LogWarning($"{LOG}: cubePool is null and fallbackPrefab not assigned.");
                return null;
            }
            cube = Instantiate(fallbackPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        if (cube == null)
        {
            return null;
        }

        var pooledObject = cube.GetComponent<PooledObject>();
        if (pooledObject == null)
        {
            pooledObject = cube.AddComponent<PooledObject>();
            if (cubePool != null)
            {
                pooledObject.OwnerPool = cubePool;
            }
        }
        else
        {
            if (pooledObject.OwnerPool == null && cubePool != null)
            {
                pooledObject.OwnerPool = cubePool;
            }
        }

        var cubeMergeHandler = cube.GetComponent<CubeMergeHandler>();
        if (cubeMergeHandler != null)
        {
            cubeMergeHandler.ResetMergeState();
        }

        CubeSpawnHelper.ApplyInitialLevel(cube);

        var rigidbody = cube.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        return cube;
    }

    public static void Release(GameObject cube)
    {
        if (cube == null)
        {
            return;
        }

        var pooledObject = cube.GetComponent<PooledObject>();
        if (pooledObject != null && pooledObject.OwnerPool != null)
        {
            pooledObject.OwnerPool.Despawn(cube);
            return;
        }
    }
}