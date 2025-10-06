using UnityEngine;

public class GameOverState : IGameState
{
    private readonly GameStateContext GameStateContext;

    public GameOverState(GameStateContext gameStateContext)
    {
        GameStateContext = gameStateContext;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;

        GameStateContext.GameOverUI.ShowGameOverScreen(0.8f);
    }

    public void ExitState()
    {
        GameStateContext.GameOverUI.HideScreen();
    }

    public void UpdateState()
    {
    }
}
