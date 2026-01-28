using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    private readonly List<AIEnemy> enemies = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SubscribeEnemy(AIEnemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void SetAllEnemiesActive(bool active)
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(active);
        }
    }
}
