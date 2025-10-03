using UnityEngine;

[DisallowMultipleComponent]
public sealed class SlingshotController : MonoBehaviour
{
    private const string LOG = "SlingshotController";

    [Header("References")]
    [SerializeField] private SlingshotView slingshotView;
    [SerializeField] private MonoBehaviour iPointerInputSource;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private SlingshotAimer slingshotAimer;
    [SerializeField] private SlingshotLauncher slingshotLauncher;

    [Header("Flow")]
    [SerializeField] private float respawnDelay = 0.2f;

    private IPointerInput iInput;
    private readonly SlingshotModel slingshotModel = new SlingshotModel();

    private void Awake()
    {
        if (slingshotView == null)
        {
            Debug.LogError($"{LOG}: View is not assigned!");
        }

        if (cubeSpawner == null)
        {
            Debug.LogError($"{LOG}: CubeSpawner is not assigned!");
        }

        if (slingshotAimer == null)
        {
            Debug.LogError($"{LOG}: SlingshotAimer is not assigned!");
        }

        if (slingshotLauncher == null)
        {
            Debug.LogError($"{LOG}: SlingshotLauncher is not assigned!");
        }

        iInput = iPointerInputSource as IPointerInput;
        if (iInput == null)
        {
            Debug.LogError($"{LOG}: pointerInputSource must implement IPointerInput!");
        }
    }

    private void Start()
    {
        SpawnReadyCube();
    }

    private void Update()
    {
        if (slingshotView == null || iInput == null)
        {
            return;
        }

        switch (slingshotModel.State)
        {
            case SlingshotState.Idle:
                if (iInput.IsDown() && PointerHitsCurrentCube())
                {
                    slingshotModel.State = SlingshotState.Aiming;
                }
                break;

            case SlingshotState.Aiming:
                if (iInput.IsHeld())
                {
                    slingshotAimer.UpdateAiming(iInput.GetScreenPosition(), slingshotModel.currentCube);
                }
                else if (iInput.IsUp())
                {
                    FireAndScheduleRespawn();
                }
                break;
        }
    }

    private void SpawnReadyCube()
    {
        if (cubeSpawner == null || slingshotAimer == null)
        {
            return;
        }

        var spawnPoint = slingshotAimer.GetSpawnPointTransform();
        if (spawnPoint == null)
        {
            Debug.LogWarning($"{LOG}: spawnPoint is null.");
            return;
        }

        var cubeGameObject = cubeSpawner.SpawnAt(spawnPoint);
        if (cubeGameObject == null)
        {
            return;
        }

        var rigidbody = cubeGameObject.GetComponent<Rigidbody>();
        slingshotModel.currentCube = cubeGameObject;
        slingshotModel.currentRigidbody = rigidbody;
        slingshotModel.State = SlingshotState.Idle;
    }

    private void FireAndScheduleRespawn()
    {
        if (slingshotModel.currentCube == null || slingshotModel.currentRigidbody == null)
        {
            return;
        }

        bool launched = slingshotLauncher.Launch(slingshotModel.currentRigidbody,
            slingshotAimer.GetSpawnPointPosition(), slingshotModel.currentCube.transform.position);

        if (launched)
        {
            slingshotModel.Clear();
            Invoke(nameof(SpawnReadyCube), respawnDelay);
        }
    }

    private bool PointerHitsCurrentCube()
    {
        if (slingshotModel.currentCube == null || slingshotView == null || slingshotView.MainCamera == null)
        {
            return false;
        }

        Ray ray = slingshotView.MainCamera.ScreenPointToRay(iInput.GetScreenPosition());
        if (Physics.Raycast(ray, out var hit, 1000f, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.collider != null && hit.collider.transform.
                IsChildOf(slingshotModel.currentCube.transform);
        }

        return false;
    }
}