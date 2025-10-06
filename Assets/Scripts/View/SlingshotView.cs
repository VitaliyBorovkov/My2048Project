using UnityEngine;

[DisallowMultipleComponent]
public sealed class SlingshotView : MonoBehaviour
{
    private const string LOG = "SlingshotView";

    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Camera mainCamera;

    [Header("Debug")]
    [SerializeField] private bool drawAimLine = true;

    public Transform SpawnPoint => spawnPoint;
    public Camera MainCamera => mainCamera;

    private void Reset()
    {
        mainCamera = Camera.main;
    }

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (spawnPoint == null)
        {
            Debug.LogError($"{LOG}: SpawnPoint is not assigned!");
        }
    }

    public void SetCubePosition(GameObject cube, Vector3 worldPosition)
    {
        if (cube == null)
        {
            return;
        }

        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // MovePosition безопаснее для объектов с Rigidbody, Unity применит это в FixedUpdate
            try
            {
                rb.MovePosition(worldPosition);
                return;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"{LOG}: MovePosition failed: {ex.Message}. Fallback to transform.position.");
            }
        }

        cube.transform.position = worldPosition;
    }

    public void DrawLine(Vector3 from, Vector3 to)
    {
        if (!drawAimLine)
        {
            return;
        }

        Debug.DrawLine(from, to, Color.red, 0f, false);
    }
}