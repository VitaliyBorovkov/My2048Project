using TMPro;

using UnityEngine;

public class CubeValueFaces : MonoBehaviour
{
    private const string LOG = "CubeValueFaces";

    [Header("References")]
    [SerializeField] private CubeLevel cubeLevel;

    [Header("Text settings")]
    [SerializeField] private TMP_Text[] faceTexts = new TMP_Text[0];

    [Header("Config")]
    [SerializeField] private CubeValueConfig cubeValueConfig;


    private void Awake()
    {
        if (cubeLevel == null)
        {
            cubeLevel = GetComponentInParent<CubeLevel>();
            if (cubeLevel == null)
            {
                Debug.LogWarning($"{LOG}: CubeLevel not assigned and not found on parents of {gameObject.name}.");
            }

        }

        if (faceTexts == null || faceTexts.Length == 0)
        {
            faceTexts = GetComponentsInChildren<TMP_Text>(true);
            if (faceTexts == null || faceTexts.Length == 0)
            {
                Debug.LogWarning($"{LOG}: Face texts not assigned on {gameObject.name}.");
            }
        }
    }

    private void OnEnable()
    {
        if (cubeLevel != null)
        {
            cubeLevel.OnLevelChanged += HandleLevelChanged;
        }

        UpdateDisplay(cubeLevel != null ? cubeLevel.Level : 1);
    }

    private void OnDisable()
    {
        if (cubeLevel != null)
        {
            cubeLevel.OnLevelChanged -= HandleLevelChanged;
        }
    }

    private void HandleLevelChanged(int newLevel)
    {
        UpdateDisplay(newLevel);
    }

    private void UpdateDisplay(int level)
    {
        string text = cubeValueConfig != null ? cubeValueConfig.GetDisplayText(level) :
            (1 << level).ToString();
        float fontSize = cubeValueConfig != null ? cubeValueConfig.GetFontSize(level) : 0f;

        if (faceTexts != null)
        {
            for (int i = 0; i < faceTexts.Length; i++)
            {
                var faceText = faceTexts[i];
                if (faceText == null)
                {
                    continue;
                }

                if (faceText.text != text)
                {
                    faceText.text = text;
                }

                if (fontSize > 0f && !Mathf.Approximately(faceText.fontSize, fontSize))
                {
                    faceText.fontSize = fontSize;
                }
            }
        }
    }
}

