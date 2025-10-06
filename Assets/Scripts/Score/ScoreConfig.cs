using UnityEngine;

[CreateAssetMenu(menuName = "Game/Score/ScoreConfig", fileName = "ScoreConfig")]
public class ScoreConfig : ScriptableObject
{
    public int[] pointsByLevel;
}