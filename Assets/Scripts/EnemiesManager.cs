using System;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public Action EnemyDiedEvent;
    public int EnemiesCount => _enemies.Length;
    
    [SerializeField] 
    private Enemy[] _enemies;

    private void Awake()
    {
        foreach (var enemy in _enemies)
        {
            enemy.DiedEvent += OnEnemyDied;
        }
    }

    private void OnEnemyDied()
    {
        EnemyDiedEvent?.Invoke();
    }

    private void OnDestroy()
    {
        foreach (var enemy in _enemies)
        {
            enemy.DiedEvent -= OnEnemyDied;
        }
    }
}