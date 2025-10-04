using System.Collections.Generic;

using UnityEngine;

public sealed class CubePool : MonoBehaviour
{
    private const string LOG = "CubePool";

    [Header("Prefab")]
    [SerializeField] private GameObject cubePrefab;

    [Header("Pool settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private bool allowGrowth = true;
    [SerializeField] private Transform poolRoot;
    [SerializeField] private int extraObjectsInPool = 0;

    [Header("RigidbodyHandler")]
    [SerializeField] private RigidbodyHandler rigidbodyHandler;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (cubePrefab == null)
        {
            Debug.LogError($"{LOG}: Prefab is not assigned on {gameObject.name}.");
            return;
        }

        if (poolRoot == null)
        {
            poolRoot = new GameObject($"{gameObject.name}_PoolRoot").transform;
            poolRoot.SetParent(transform, false);
        }

        PreparingPool(initialPoolSize);

        if (extraObjectsInPool > 0)
        {
            ExpandPool(extraObjectsInPool);
        }
    }
    public void PreparingPool(int count)
    {
        if (cubePrefab == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var cube = Instantiate(cubePrefab, poolRoot);
            AttachPooledObject(cube);
            PreparePooledObject(cube);
            CubeResetter.ResetState(cube);
            pool.Enqueue(cube);
        }
    }

    public void ExpandPool(int count)
    {
        if (count <= 0) return;
        PreparingPool(count);
        Debug.Log($"{LOG}: Expanded pool by {count}. New size = {pool.Count}.");
    }

    public void AttachPooledObject(GameObject cube)
    {
        PooledObjectAttacher.Attach(cube, this);
    }

    private void PreparePooledObject(GameObject cube)
    {
        cube.SetActive(false);
        cube.transform.SetParent(poolRoot, false);
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject cube = null;

        if (pool.Count > 0)
        {
            cube = pool.Dequeue();
            cube.transform.SetParent(null, true);
        }
        else
        {
            if (allowGrowth)
            {
                cube = Instantiate(cubePrefab, poolRoot);
                AttachPooledObject(cube);
                Debug.Log($"{LOG}: Pool empty - instantiated new object due allowGrowth.");
            }
            else
            {
                Debug.LogWarning($"{LOG}: Pool is empty and growth is not allowed.");
                return null;
            }
        }

        cube.transform.position = position;
        cube.transform.rotation = rotation;
        cube.SetActive(true);

        rigidbodyHandler.ResetVelocity(cube);

        CubeResetter.ResetState(cube);

        return cube;
    }

    public void Despawn(GameObject cube)
    {
        if (cube == null)
        {
            Debug.LogWarning($"{LOG}: Attempted to despawn a null object.");
            return;
        }

        rigidbodyHandler.ResetVelocity(cube);

        CubeResetter.ResetState(cube);

        cube.SetActive(false);
        cube.transform.SetParent(poolRoot, false);
        pool.Enqueue(cube);
    }
}
