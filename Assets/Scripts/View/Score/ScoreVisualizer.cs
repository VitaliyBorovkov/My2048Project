using TMPro;

using UnityEngine;

public sealed class ScoreVisualizer : MonoBehaviour
{
    private const string LOG = "ScoreVisualizer";

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("References")]
    [SerializeField] private ScoreService scoreService;

    private void Awake()
    {
        if (scoreService == null)
        {
            Debug.LogWarning($"{LOG}: ScoreService is not assigned on {gameObject.name}. Attempting to find in scene.");
        }
    }

    private void OnEnable()
    {
        if (scoreService != null)
        {
            scoreService.OnScoreChanged += OnScoreChanged;
            UpdateDisplay(scoreService.TotalScore);
        }
    }

    private void OnDisable()
    {
        if (scoreService != null)
        {
            scoreService.OnScoreChanged -= OnScoreChanged;
        }
    }

    private void OnScoreChanged(int totalScore)
    {
        UpdateDisplay(totalScore);
    }

    private void UpdateDisplay(int totalScore)
    {
        if (messageText != null)
        {
            messageText.text = totalScore.ToString();
            messageText.alpha = 1f;
        }
    }
}