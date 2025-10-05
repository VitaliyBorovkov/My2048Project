using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    private const string LOG = "GameStateMachine";

    [Header("Screens")]
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PauseUI pauseUI;

    private GameStateContext gameStateContext;
    private GameStateController gameStateController;

    private void Awake()
    {
        if (pauseUI == null)
        {
            Debug.LogWarning($"{LOG}: PauseUI is not assigned in inspector.");
        }

        if (gameOverUI == null)
        {
            Debug.LogWarning($"{LOG}: GameOverUI is not assigned in inspector.");
        }

        gameStateContext = new GameStateContext(gameOverUI, pauseUI);
        gameStateController = new GameStateController(gameStateContext);

        pauseUI?.HideScreen();
        gameOverUI?.HideScreen();
    }

    private void Start()
    {
        gameStateController.ToGameplay();
    }

    private void Update()
    {
        gameStateController.Update();
    }

    public void ToGameplay()
    {
        gameStateController.ToGameplay();
    }

    public void ToGameOver()
    {
        gameStateController.ToGameOver();
        Debug.Log($"{LOG}: ToGameOver requested.");
    }

    public void ToPause()
    {
        gameStateController.ToPause();
    }

    public void SetCursor(bool show)
    {
        gameStateContext.SetCursor(show);
    }
}