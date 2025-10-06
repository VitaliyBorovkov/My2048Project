using UnityEngine;
using UnityEngine.UI;

public class GameStateMachine : MonoBehaviour
{
    private const string LOG = "GameStateMachine";

    [Header("Screens")]
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PauseUI pauseUI;

    [Header("Pause button")]
    [SerializeField] private Button pauseButton;

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

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }
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
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }
    }

    public void ToGameOver()
    {
        gameStateController.ToGameOver();
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }

    public void ToPause()
    {
        gameStateController.ToPause();
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }
}