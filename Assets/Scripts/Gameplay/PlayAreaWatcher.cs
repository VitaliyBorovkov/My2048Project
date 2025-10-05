using System.Collections.Generic;

using UnityEngine;

public class PlayAreaWatcher : MonoBehaviour
{
    private const string LOG = "PlayAreaWatcher";

    [SerializeField] private string cubeTag = "Cube";

    private readonly List<IPlayAreaObserver> observers = new List<IPlayAreaObserver>();

    private bool IsCubeCandidate(GameObject cube)
    {
        if (string.IsNullOrEmpty(cubeTag))
        {
            return false;
        }
        return cube.CompareTag(cubeTag);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!IsCubeCandidate(collider.gameObject))
        {
            return;
        }

        if (!collider.TryGetComponent<PlayableCube>(out var playableCube))
        {
            playableCube = collider.gameObject.AddComponent<PlayableCube>();
            Debug.Log($"{LOG}: Marked playable -> {collider.gameObject.name}");
        }
        playableCube.Mark();
        NotifyMarked(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!IsCubeCandidate(collider.gameObject))
        {
            return;
        }

        if (collider.TryGetComponent<PlayableCube>(out var playableCube))
        {
            playableCube.Unmark();
            NotifyUnmarked(collider.gameObject);
        }
    }

    public void RegisterObserver(IPlayAreaObserver iPlayAreaObserver)
    {
        if (iPlayAreaObserver == null)
        {
            return;
        }

        if (observers.Contains(iPlayAreaObserver))
        {
            return;
        }

        observers.Add(iPlayAreaObserver);
    }

    public void UnregisterObserver(IPlayAreaObserver iPlayAreaObserver)
    {
        if (iPlayAreaObserver == null)
        {
            return;
        }

        observers.Remove(iPlayAreaObserver);
    }

    private void NotifyMarked(GameObject cube)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].OnMarkedPlayable(cube);
        }
    }

    private void NotifyUnmarked(GameObject cube)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].OnUnmarkedPlayable(cube);
        }
    }
}
