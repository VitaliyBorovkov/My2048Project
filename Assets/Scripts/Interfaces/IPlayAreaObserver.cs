using UnityEngine;

public interface IPlayAreaObserver
{
    void OnMarkedPlayable(GameObject playable);

    void OnUnmarkedPlayable(GameObject playable);
}
