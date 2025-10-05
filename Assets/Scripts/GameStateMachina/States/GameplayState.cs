using UnityEngine;

public sealed class GameplayState : IGameState
{
    private const string LOG = "GameplayState";

    private readonly GameStateContext GameStateContext;

    public GameplayState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 1f;
        GameStateContext.PauseUI?.HideScreen();
        GameStateContext.GameOverUI?.HideScreen();
        GameStateContext.SetCursor(false);
    }

    public void ExitState()
    {
    }

    public void UpdateState()
    {
    }
}
