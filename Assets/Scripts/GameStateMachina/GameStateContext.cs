public sealed class GameStateContext
{
    public readonly GameOverUI GameOverUI;
    public readonly PauseUI PauseUI;

    public GameStateContext(GameOverUI gameOverUI, PauseUI pauseUI)
    {
        GameOverUI = gameOverUI;
        PauseUI = pauseUI;
    }
}
