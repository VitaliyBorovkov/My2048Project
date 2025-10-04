using System;

using UnityEngine;

public sealed class PointerInput : MonoBehaviour, IPointerInput
{
    public event Action<Vector3> OnDown;
    public event Action<Vector3> OnHold;
    public event Action<Vector3> OnUp;

    private Vector3 lastScreenPosition;

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            lastScreenPosition = Input.mousePosition;
            OnDown?.Invoke(lastScreenPosition);
            return;
        }
        if (Input.GetMouseButton(0))
        {
            lastScreenPosition = Input.mousePosition;
            OnHold?.Invoke(lastScreenPosition);
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            lastScreenPosition = Input.mousePosition;
            OnUp?.Invoke(lastScreenPosition);
            return;
        }
#else
        if (Input.touchCount == 0) 
        {
            return;
        }
        var t = Input.GetTouch(0);
        lastScreenPosition = t.position;
        switch (t.phase)
        {
            case TouchPhase.Began:      OnDown?.Invoke(lastScreenPosition); break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary: OnHold?.Invoke(lastScreenPosition); break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:   OnUp?.Invoke(lastScreenPosition); break;
        }
#endif
    }

    public Vector3 GetScreenPosition()
    {
        if (lastScreenPosition == default(Vector3))
        {
            return new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        }
        return lastScreenPosition;
    }
}
