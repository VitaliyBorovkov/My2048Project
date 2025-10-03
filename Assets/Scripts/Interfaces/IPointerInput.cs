using UnityEngine;

public interface IPointerInput
{
    bool IsDown();
    bool IsHeld();
    bool IsUp();
    Vector3 GetScreenPosition();
}
