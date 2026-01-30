using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    private readonly List<AIEnemy> enemies = new();

    private AIEnemy activeChasingEnemy;
    private AIEnemy inactiveEnemy;

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

    public void RegisterEnemy(AIEnemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void TryActivateEnemy(AIEnemy enemy)
    {
        if (activeChasingEnemy != null)
            return;

        if (inactiveEnemy != null)
        {
            inactiveEnemy.SetState(IAState.Imitating);
            inactiveEnemy = null;
        }

        activeChasingEnemy = enemy;

        foreach (var e in enemies)
        {
            if (e == enemy)
                e.SetState(IAState.Chasing);
            else
                e.SetState(IAState.InactiveImitating);
        }
    }

    public void SetEnemyInactive(AIEnemy enemy)
    {
        if (enemy != activeChasingEnemy)
            return;

        enemy.SetState(IAState.Inactive);
        inactiveEnemy = enemy;
        activeChasingEnemy = null;

        foreach (var e in enemies)
        {
            if (e != inactiveEnemy)
                e.SetState(IAState.Imitating);
        }
    }

    public void OnPlayerEnteredSafeZone()
    {
        if (activeChasingEnemy != null)
        {
            activeChasingEnemy.ResetToInitialState();
            activeChasingEnemy = null;
        }

        inactiveEnemy = null;

        foreach (var e in enemies)
        {
            if (e.State != IAState.Imitating)
                e.SetState(IAState.Imitating);
        }
    }
}
public enum IAState
{
    Imitating,
    InactiveImitating,
    Inactive,
    Chasing
}
