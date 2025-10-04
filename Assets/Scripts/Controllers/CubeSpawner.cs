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

        PooledObjectAttacher.Attach(cube, cubePool);
        CubeResetter.ResetState(cube);
        PhysicsResetter.ResetVelocity(cube);
        CubeSpawnHelper.ApplyInitialLevel(cube);

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