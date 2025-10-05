using UnityEngine;

public class GameOverState : IGameState
{
    private const string LOG = "GameOverState";

    private readonly GameStateContext GameStateContext;

    public GameOverState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;

        GameStateContext.GameOverUI.ShowGameOverScreen(0.8f);
        GameStateContext.SetCursor(true);
        Debug.Log($"{LOG}: Entered.");
    }

    public void ExitState()
    {
        GameStateContext.GameOverUI.HideScreen();
        Debug.Log($"{LOG}: Exited.");
    }

    public void UpdateState()
    {
    }
}
