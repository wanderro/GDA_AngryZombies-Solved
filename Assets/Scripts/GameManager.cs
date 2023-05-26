using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private EnemiesManager _enemiesManager;
    [SerializeField] 
    private ScoreManager _scoreManager;
    
    private void Start()
    {
        _enemiesManager.EnemyDiedEvent += _scoreManager.DecreaseEnemiesCount;
        _scoreManager.Initialize(_enemiesManager.EnemiesCount);
    }

    private void OnDestroy()
    {
        _enemiesManager.EnemyDiedEvent -= _scoreManager.DecreaseEnemiesCount;
    }
}