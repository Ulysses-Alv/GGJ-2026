using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    private readonly List<AIEnemy> mainEnemies = new();
    private readonly List<AIEnemy> hallwayEnemies = new();

    private List<AIEnemy> currentEnemies;

    private AIEnemy activeChasingEnemy;
    private AIEnemy inactiveEnemy;

    private bool canActivateAny;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void EnableActivation()
    {
        canActivateAny = true;
    }

    public void RegisterEnemy(AIEnemy enemy, bool isMainZone)
    {
        var list = isMainZone ? mainEnemies : hallwayEnemies;

        if (!list.Contains(enemy))
            list.Add(enemy);

    }
    public enum Reasons
    {
        No_CantActivate,
        No_ActiveChasing,
        No_WrongZone,
        Yes_Correct
    }
    public Reasons TryActivateEnemy(AIEnemy enemy)
    {
        if (!canActivateAny)
            return Reasons.No_CantActivate;

        if (activeChasingEnemy != null)
            return Reasons.No_ActiveChasing;

        if (currentEnemies == null || !currentEnemies.Contains(enemy))
            return Reasons.No_WrongZone;

        if (inactiveEnemy != null)
        {
            inactiveEnemy.SetState(IAState.Imitating);
            inactiveEnemy = null;
        }

        activeChasingEnemy = enemy;

        foreach (var e in currentEnemies)
        {
            if (e == enemy)
            {
                Debug.Log("Activating Statue");
                e.SetState(IAState.Chasing);
            }
            else
            {
                e.SetState(IAState.InactiveImitating);

            }
        }
        return Reasons.Yes_Correct;
    }

    public void SetEnemyInactive(AIEnemy enemy)
    {
        if (enemy != activeChasingEnemy)
            return;

        enemy.SetState(IAState.Inactive);
        inactiveEnemy = enemy;
        activeChasingEnemy = null;

        foreach (var e in currentEnemies)
        {
            if (e != inactiveEnemy)
                e.SetState(IAState.Imitating);
        }
    }

    public void SetPlayerZone(GameZones zone)
    {
        switch (zone)
        {
            case GameZones.SafeZone:
                ResetAllEnemies();
                break;
            case GameZones.Electricity:
                ResetAllEnemies();
                break;

            case GameZones.Main:
                Debug.Log("Current Zone: Main");
                SwitchZone(mainEnemies);
                break;

            case GameZones.HallwayEnd:
                Debug.Log("Current Zone: Hallway");
                SwitchZone(hallwayEnemies);
                break;
        }
    }

    private void SwitchZone(List<AIEnemy> newZoneEnemies)
    {
        ResetAllEnemies();
        currentEnemies = newZoneEnemies;
    }

    private void ResetAllEnemies()
    {
        if (activeChasingEnemy != null)
            activeChasingEnemy.ResetToInitialState();

        activeChasingEnemy = null;
        inactiveEnemy = null;

        foreach (var e in mainEnemies)
            e.SetState(IAState.Imitating);

        foreach (var e in hallwayEnemies)
            e.SetState(IAState.Imitating);
    }
}

public enum IAState
{
    Imitating,
    InactiveImitating,
    Inactive,
    Chasing
}
