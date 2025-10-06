using System.Collections;

using UnityEngine;

[DisallowMultipleComponent]
public sealed class SlingshotController : MonoBehaviour
{
    private const string LOG = "SlingshotController";

    [Header("References")]
    [SerializeField] private SlingshotView slingshotView;
    [SerializeField] private MonoBehaviour iPointerInput;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private SlingshotAimer slingshotAimer;
    [SerializeField] private SlingshotLauncher slingshotLauncher;

    [Header("Flow")]
    [SerializeField] private float respawnDelay = 0.2f;
    [SerializeField] private LayerMask pickMask;
    [SerializeField] private float maxRayDistance = 1000f;

    private IPointerInput iInput;
    private Coroutine respawnCoroutine;
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

        iInput = iPointerInput as IPointerInput;
        if (iInput == null)
        {
            Debug.LogError($"{LOG}: pointerInputSource must implement IPointerInput!");
        }
    }

    private void OnEnable()
    {
        if (iInput != null)
        {
            iInput.OnDown += HandlePointerDown;
            iInput.OnHold += HandlePointerHold;
            iInput.OnUp += HandlePointerUp;
        }

        CancelPendingRespawn();
    }

    private void OnDisable()
    {
        if (iInput != null)
        {
            iInput.OnDown -= HandlePointerDown;
            iInput.OnHold -= HandlePointerHold;
            iInput.OnUp -= HandlePointerUp;
        }

        CancelPendingRespawn();
    }

    private void Start()
    {
        SpawnReadyCube();
    }

    //private void Update()
    //{
    //    if (slingshotView == null || iInput == null)
    //    {
    //        return;
    //    }

    //    switch (slingshotModel.State)
    //    {
    //        case SlingshotState.Idle:
    //            if (iInput.IsDown() && PointerHitsCurrentCube())
    //            {
    //                slingshotModel.State = SlingshotState.Aiming;
    //            }
    //            break;

    //        case SlingshotState.Aiming:
    //            if (iInput.IsHeld())
    //            {
    //                slingshotAimer.UpdateAiming(iInput.GetScreenPosition(), slingshotModel.currentCube);
    //            }
    //            else if (iInput.IsUp())
    //            {
    //                FireAndScheduleRespawn();
    //            }
    //            break;
    //    }
    //}

    private void HandlePointerDown(Vector3 screenPosition)
    {
        if (slingshotModel.State != SlingshotState.Idle)
            return;

        if (PointerHitsCurrentCube(screenPosition))
        {
            slingshotModel.State = SlingshotState.Aiming;
        }
    }

    private void HandlePointerHold(Vector3 screenPosition)
    {
        if (slingshotModel.State != SlingshotState.Aiming)
            return;

        if (slingshotAimer != null)
        {
            slingshotAimer.UpdateAiming(screenPosition, slingshotModel.currentCube);
        }
    }

    private void HandlePointerUp(Vector3 screenPosition)
    {
        if (slingshotModel.State != SlingshotState.Aiming)
            return;

        FireAndScheduleRespawn();
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
            ScheduleRespawn(respawnDelay);
        }
        else
        {
            slingshotModel.State = SlingshotState.Idle;
        }
    }

    private void ScheduleRespawn(float delay)
    {
        CancelPendingRespawn();
        respawnCoroutine = StartCoroutine(RespawnCoroutine(delay));
    }

    private void CancelPendingRespawn()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }
    }

    private IEnumerator RespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
            yield break;

        SpawnReadyCube();
        respawnCoroutine = null;
    }

    private bool PointerHitsCurrentCube(Vector3 screenPosition)
    {
        if (slingshotModel.currentCube == null || slingshotView == null || slingshotView.MainCamera == null)
        {
            return false;
        }

        Ray ray = slingshotView.MainCamera.ScreenPointToRay(iInput.GetScreenPosition());
        if (Physics.Raycast(ray, out var hit, maxRayDistance, pickMask, QueryTriggerInteraction.Ignore))
        {
            return hit.collider != null && hit.collider.transform.
                IsChildOf(slingshotModel.currentCube.transform);
        }

        return false;
    }
}