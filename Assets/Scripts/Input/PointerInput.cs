using UnityEngine;

public sealed class PointerInput : MonoBehaviour, IPointerInput
{
    public bool IsDown()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
#endif
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return true;
        }
        return false;
    }

    public bool IsHeld()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            return true;
        }
#endif
        if (Input.touchCount > 0)
        {
            var phase = Input.GetTouch(0).phase;
            if (phase == TouchPhase.Moved || phase == TouchPhase.Stationary)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsUp()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(0))
        {
            return true;
        }
#endif
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            return true;
        }
        return false;
    }

    public Vector3 GetScreenPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.mousePosition;
#else
        return Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position
            : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
#endif
    }
}
