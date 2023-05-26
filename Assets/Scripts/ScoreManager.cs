using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public UnityEvent<int> IncreaseScoreEvent;
    public UnityEvent GameWinEvent;
    private int _zombiesCount;

    public void Initialize(int zombiesCount)
    {
        _zombiesCount = zombiesCount;
    }

    public void DecreaseEnemiesCount()
    {
        IncreaseScoreEvent.Invoke(Mathf.Max(--_zombiesCount, 0));
        
        if (_zombiesCount <= 0)
        {
            GameWinEvent.Invoke();
        }
    }
}