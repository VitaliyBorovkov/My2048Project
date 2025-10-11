using System;

using UnityEngine;

public sealed class FilteredPointerInput : MonoBehaviour, IPointerInput
{
    private const string LOG = "FilteredPointerInput";

    [Header("Raw input")]
    [SerializeField] private MonoBehaviour rawPointerInput;

    [Header("Raycast / filter settings")]
    [SerializeField] private Camera overrideCamera;
    [SerializeField] private LayerMask blockingRaycastMask;
    [SerializeField] private float rayMaxDistance = 1000f;

    [Header("Gesture thresholds")]
    [SerializeField, Min(0f)] private float dragThresholdPixels = 8f;
    [SerializeField, Min(0f)] private float holdThresholdSeconds = 0.12f;

    public event Action<Vector3> OnDown;
    public event Action<Vector3> OnHold;
    public event Action<Vector3> OnUp;

    private IPointerInput rawInput;
    private bool pointerDown;
    private Vector3 pointerStartPos;
    private float pointerStartTime;
    private bool dragStarted;
    private bool dragStartedByHold;
    private bool startedOverGameObject;
    private Vector3 lastScreenPosition;

    private RaycastHit lastDownHit;
    private bool hasLastDownHit;

    private void Awake()
    {
        if (rawPointerInput == null)
        {
            Debug.LogError($"{LOG}: RawPointerInput is not assigned!");
            return;
        }

        rawInput = rawPointerInput as IPointerInput;
        if (rawInput == null)
        {
            Debug.LogError($"{LOG}: RawPointerInput does not implement IPointerInput.");
        }
    }

    private void OnEnable()
    {
        if (rawInput != null)
        {
            rawInput.OnDown += HandleRawDown;
            rawInput.OnHold += HandleRawHold;
            rawInput.OnUp += HandleRawUp;
        }
    }

    private void OnDisable()
    {
        if (rawInput != null)
        {
            rawInput.OnDown -= HandleRawDown;
            rawInput.OnHold -= HandleRawHold;
            rawInput.OnUp -= HandleRawUp;
        }
    }

    private void HandleRawDown(Vector3 screenPosition)
    {
        lastScreenPosition = screenPosition;
        pointerDown = true;
        pointerStartPos = screenPosition;
        pointerStartTime = Time.unscaledTime;
        dragStarted = false;
        dragStartedByHold = false;
        hasLastDownHit = false;
        startedOverGameObject = false;

        Camera camera = overrideCamera != null ? overrideCamera : Camera.main;
        startedOverGameObject = false;
        if (camera != null)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out var hit, rayMaxDistance, blockingRaycastMask, QueryTriggerInteraction.Ignore))
            {
                startedOverGameObject = hit.collider != null;
                lastDownHit = hit;
                hasLastDownHit = hit.collider != null;
            }
            else
            {
                hasLastDownHit = false;
            }
        }

        //Debug.Log($"{LOG}: RawDown at {screenPosition}, startedOverGameObject={startedOverGameObject}");
        OnDown?.Invoke(screenPosition);
    }

    private void HandleRawHold(Vector3 screenPosition)
    {
        lastScreenPosition = screenPosition;

        if (!pointerDown)
        {
            OnHold?.Invoke(screenPosition);
            return;
        }

        var delta = screenPosition - pointerStartPos;
        float distance = delta.magnitude;
        float elapsed = Time.unscaledTime - pointerStartTime;

        if (!dragStarted)
        {
            if (distance >= dragThresholdPixels)
            {
                dragStarted = true;
                dragStartedByHold = false;
                //Debug.Log($"{LOG}: Drag started by movement (dist={dist:F1}px).");
            }
            else if (elapsed >= holdThresholdSeconds)
            {
                dragStarted = true;
                dragStartedByHold = true;
                //Debug.Log($"{LOG}: Drag started by hold threshold (elapsed={elapsed:F2}s).");
            }
        }
        OnHold?.Invoke(screenPosition);
    }

    private void HandleRawUp(Vector3 screenPosition)
    {
        lastScreenPosition = screenPosition;

        if (!pointerDown)
        {
            OnUp?.Invoke(screenPosition);
            return;
        }

        pointerDown = false;
        var delta = screenPosition - pointerStartPos;
        float distance = delta.magnitude;
        float elapsed = Time.unscaledTime - pointerStartTime;

        //Debug.Log($"{LOG}: RawUp at {screenPosition}, dragStarted={dragStarted}, startedOverGameObject={startedOverGameObject}, dist={dist:F1}, elapsed={elapsed:F2}s");

        bool validDragRelease = dragStarted && distance >= dragThresholdPixels;

        if (startedOverGameObject && !validDragRelease)
        {
            string reason = !dragStarted ? "no drag started" : $"dragStartedByHold={(dragStartedByHold ? "yes" : "no")} but move insufficient ({distance:F1}px < {dragThresholdPixels}px)";
            //Debug.Log($"{LOG}: Up ignored, {reason}.");
            return;
        }

        OnUp?.Invoke(screenPosition);
    }

    public Vector3 GetScreenPosition()
    {
        if (rawInput != null)
        {
            return rawInput.GetScreenPosition();
        }

        if (lastScreenPosition == default(Vector3))
        {
            return new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        }
        return lastScreenPosition;
    }

    public bool TryGetLastPointerHit(out RaycastHit hit)
    {
        if (hasLastDownHit)
        {
            hit = lastDownHit;
            return true;
        }
        hit = default;
        return false;
    }
}
