using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private const string LOG = "ButtonHandler";

    [Header("Scene Settings")]
    [SerializeField] private GameStateMachine gameStateMachine;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button[] mainMenuButtons;

    private void Awake()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartScene);
        }

        foreach (var button in mainMenuButtons)
        {
            if (button != null)
            {
                button.onClick.AddListener(LoadMainMenu);
            }

        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(Resume);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(Settings);
        }
    }

    private void PlayGame()
    {
        //Debug.Log($"{LOG}: PlayGame clicked.");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Resume()
    {
        //Debug.Log($"{LOG}: Resume button clicked.");
        gameStateMachine?.ToGameplay();
    }

    public void Settings()
    {
        Debug.Log($"{LOG}: Settings button clicked.");
    }

    public void RestartScene()
    {
        //Debug.Log($"{LOG}: Restart current scene.");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        //Debug.Log($"{LOG}: Load Main Menu.");
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning($"{LOG}: Main menu scene name is not set!");
        }
    }

    public void QuitGame()
    {
        //Debug.Log($"{LOG}: Quit button clicked.");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
