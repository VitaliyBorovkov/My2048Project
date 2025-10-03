using UnityEngine;

public sealed class SlingshotModel
{
    public SlingshotState State = SlingshotState.Idle;

    public GameObject currentCube;
    public Rigidbody currentRigidbody;

    public void Clear()
    {
        currentCube = null;
        currentRigidbody = null;
        State = SlingshotState.Shooting;
    }
}