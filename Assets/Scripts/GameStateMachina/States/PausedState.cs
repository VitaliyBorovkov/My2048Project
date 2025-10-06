using UnityEngine;

public sealed class PausedState : IGameState
{
    private readonly GameStateContext GameStateContext;

    public PausedState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;
        GameStateContext.PauseUI?.ShowScreen();
    }

    public void ExitState()
    {
        GameStateContext.PauseUI?.HideScreen();
        Time.timeScale = 1f;
    }

    public void UpdateState()
    {
    }
}
