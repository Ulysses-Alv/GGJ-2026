using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AIEnemy : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float chaseSpeedMultiplier = 1.15f;
    [SerializeField] private float focusRequiredTime = 1.5f;

    private NavMeshAgent agent;
    private Transform player;
    private IAState currentState;

    private float focusTimer;
    private bool isFocused;

    public IAState State => currentState;

    [SerializeField] private TriggerEvent OnPlayerNear, OnPlayerKilled;

    private bool isChasing => currentState.Equals(IAState.Chasing);

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        AIManager.Instance.RegisterEnemy(this);
        SetState(IAState.Imitating);
        OnPlayerNear.unityEvent.AddListener(HandlePlayerNear);
        OnPlayerKilled.unityEvent.AddListener(HandlePlayerKilled);
    }



    void Update()
    {
        if (isChasing)
        {
            agent.SetDestination(player.position);

            if (isFocused)
            {
                focusTimer += Time.deltaTime;
                if (focusTimer >= focusRequiredTime)
                {
                    AIManager.Instance.SetEnemyInactive(this);
                }
            }
            else
            {
                focusTimer = 0f;
            }
        }
    }

    public void SetState(IAState newState)
    {
        currentState = newState;
        focusTimer = 0f;
        isFocused = false;

        switch (currentState)
        {
            case IAState.Imitating:
                agent.isStopped = true;
                break;

            case IAState.Chasing:
                agent.isStopped = false;
                agent.speed = baseSpeed * chaseSpeedMultiplier;
                break;

            case IAState.InactiveImitating:
                agent.isStopped = true;
                break;

            case IAState.Inactive:
                agent.isStopped = true;
                break;
        }
    }

    public void RequestActivation()
    {
        if (currentState == IAState.Imitating)
        {
            AIManager.Instance.TryActivateEnemy(this);
        }
    }
    private void HandlePlayerKilled()
    {
        if (isChasing)
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        throw new NotImplementedException();
    }

    private void HandlePlayerNear()
    {
        RequestActivation();
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;
    }
}
