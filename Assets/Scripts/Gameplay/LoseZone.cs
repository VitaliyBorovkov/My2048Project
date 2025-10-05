using UnityEngine;

public class LoseZone : MonoBehaviour
{
    private const string LOG = "LoseZone";

    [SerializeField] private GameStateMachine gameStateMachine;

    private bool triggered;

    private void Awake()
    {
        if (gameStateMachine == null)
        {
            gameStateMachine = FindFirstObjectByType<GameStateMachine>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (!other.TryGetComponent<PlayableCube>(out var marker) || !marker.IsPlayable)
        {
            return;
        }
        TriggerGameOver(other.gameObject);
    }

    private void TriggerGameOver(GameObject culprit)
    {
        Debug.Log($"{LOG}: playable object crossed lose zone -> '{culprit.name}'.");

        if (gameStateMachine != null)
        {
            gameStateMachine.ToGameOver();
            Debug.Log($"{LOG}: requested GameStateMachine.ToGameOver()");
        }

        triggered = true;
    }
}
