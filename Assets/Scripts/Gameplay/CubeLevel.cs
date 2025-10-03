using System;

using UnityEngine;

[DisallowMultipleComponent]
public sealed class CubeLevel : MonoBehaviour
{
    private const string LOG = "CubeLevel";

    [SerializeField, Min(1)] private int level = 1;

    [Header("Visual")]
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string colorPropertyName = "_BaseColor";

    private MaterialPropertyBlock materialProperty;
    private Color defaultBaseColor;

    public event Action<int> OnLevelChanged;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
            if (targetRenderer == null)
            {
                Debug.LogWarning($"{LOG}: Renderer not found on {gameObject.name}. Color changes won't be visible.");
            }
        }

        materialProperty = new MaterialPropertyBlock();
        ApplyVisual();
    }

    public int Level => level;

    public void SetLevel(int newLevel)
    {
        int clamped = Mathf.Max(1, newLevel);
        if (clamped == level)
        {
            return;
        }

        level = clamped;
        ApplyVisual();
        OnLevelChanged?.Invoke(level);
    }

    public void IncreaseLevel(int delta = 1)
    {
        SetLevel(level + delta);
    }

    public void SetColor(Color color)
    {
        baseColor = color;
        ApplyColor(color);
    }

    public void ResetVisualToDefault()
    {
        baseColor = defaultBaseColor;
        ApplyColor(baseColor);
    }

    private void ApplyVisual()
    {
        ApplyColor(baseColor);
    }

    private void ApplyColor(Color color)
    {
        if (targetRenderer == null || materialProperty == null)
        {
            return;
        }

        targetRenderer.GetPropertyBlock(materialProperty);

        materialProperty.SetColor(colorPropertyName, color);
        if (colorPropertyName != "_BaseColor")
        {
            materialProperty.SetColor("_BaseColor", color);
        }

        targetRenderer.SetPropertyBlock(materialProperty);
    }
}