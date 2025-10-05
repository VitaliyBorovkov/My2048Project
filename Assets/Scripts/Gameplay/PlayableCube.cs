using UnityEngine;

public class PlayableCube : MonoBehaviour
{
    [SerializeField] private bool isPlayable;

    public bool IsPlayable => isPlayable;

    public void Mark()
    {
        if (isPlayable)
        {
            return;
        }
        isPlayable = true;
    }

    public void Unmark()
    {
        if (!isPlayable)
        {
            return;
        }
        isPlayable = false;
    }

    public void ResetPlayable()
    {
        isPlayable = false;
    }

    private void OnDisable()
    {
        isPlayable = false;
    }
}
