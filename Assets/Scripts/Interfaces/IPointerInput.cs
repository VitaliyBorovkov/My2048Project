using System;

using UnityEngine;

public interface IPointerInput
{
    event Action<Vector3> OnDown;
    event Action<Vector3> OnHold;
    event Action<Vector3> OnUp;
    Vector3 GetScreenPosition();

    bool TryGetLastPointerHit(out RaycastHit hit);
}
