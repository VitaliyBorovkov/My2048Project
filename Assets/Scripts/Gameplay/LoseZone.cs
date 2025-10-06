using UnityEngine;

public class LoseZone : MonoBehaviour
{
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
        if (gameStateMachine != null)
        {
            gameStateMachine.ToGameOver();
        }

        triggered = true;
    }
}
