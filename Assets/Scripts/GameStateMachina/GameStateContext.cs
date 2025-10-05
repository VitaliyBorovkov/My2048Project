using UnityEngine;

public sealed class GameStateContext
{
    public readonly GameOverUI GameOverUI;
    public readonly PauseUI PauseUI;

    public GameStateContext(GameOverUI gameOverUI, PauseUI pauseUI)
    {
        GameOverUI = gameOverUI;
        PauseUI = pauseUI;
    }

    public void SetCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
