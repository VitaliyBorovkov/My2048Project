using UnityEngine;

public sealed class GameplayState : IGameState
{
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
    }

    public void ExitState()
    {
    }

    public void UpdateState()
    {
    }
}
