using UnityEngine;

public sealed class PausedState : IGameState
{
    private const string LOG = "PausedState";

    private readonly GameStateContext GameStateContext;

    public PausedState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;

        GameStateContext.PauseUI?.ShowScreen();
        GameStateContext.SetCursor(true);
    }

    public void ExitState()
    {
        GameStateContext.PauseUI?.HideScreen();

        Time.timeScale = 1f;
        GameStateContext.SetCursor(false);
    }

    public void UpdateState()
    {
    }
}
