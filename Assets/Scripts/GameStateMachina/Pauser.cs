using UnityEngine;
using UnityEngine.UI;

public class Pauser : MonoBehaviour
{
    private const string LOG = "Pauser";

    [Header("References")]
    [SerializeField] private GameStateMachine gameStateMachine;
    [SerializeField] private Button pauseButton;

    private bool subscribedToButton;

    private void Awake()
    {
        if (gameStateMachine == null)
        {
            gameStateMachine = FindFirstObjectByType<GameStateMachine>();
        }

        if (pauseButton == null)
        {
            Debug.LogWarning($"{LOG}: pauseButton is not assigned. Pauser will be inactive.");
            return;
        }

        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        subscribedToButton = true;
    }

    private void OnDestroy()
    {
        UnsubscribeButton();
    }

    private void UnsubscribeButton()
    {
        if (!subscribedToButton || pauseButton == null)
        {
            subscribedToButton = false;
            return;
        }

        pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        subscribedToButton = false;
    }

    private void OnPauseButtonClicked()
    {
        TogglePause();
    }

    private void TogglePause()
    {
        if (gameStateMachine != null)
        {
            if (Time.timeScale > 0.5f)
            {
                gameStateMachine.ToPause();
            }
            else
            {
                gameStateMachine.ToGameplay();
            }
            return;
        }
    }
}
