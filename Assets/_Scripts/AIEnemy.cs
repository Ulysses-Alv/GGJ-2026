using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIEnemy : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float chaseSpeedMultiplier = 1.15f;
    [SerializeField] private float focusRequiredTime = 1.5f;
    [SerializeField] private TriggerEvent killEvent, chaseEvent;
    [SerializeField] private AudioSource movementAudio;
    [SerializeField] private bool isMain;

    private NavMeshAgent agent;
    private Transform player;
    private IAState currentState;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float focusTimer;
    private bool isFocused;

    public IAState State => currentState;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        initialPosition = transform.position;
        initialRotation = transform.rotation;

    }
    private void Start()
    {
        AIManager.Instance.RegisterEnemy(this, isMain);
        SetState(IAState.Imitating);
        killEvent.unityEvent.AddListener(HandleKill);
        chaseEvent.unityEvent.AddListener(HandleChase);
    }

    private void HandleChase()
    {
        RequestActivation();
    }

    private void HandleKill()
    {
        PlayerJumpScare.instance.TriggerJumpscare();
    }

    void Update()
    {
        if (currentState == IAState.Chasing)
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
                StopMovementAudio();
                break;

            case IAState.Chasing:
                agent.isStopped = false;
                agent.speed = baseSpeed * chaseSpeedMultiplier;
                PlayMovementAudio();
                break;

            case IAState.InactiveImitating:
                agent.isStopped = true;
                StopMovementAudio();
                break;

            case IAState.Inactive:
                agent.isStopped = true;
                StopMovementAudio();
                break;
        }
    }

    private void PlayMovementAudio()
    {
        if (!movementAudio.isPlaying)
            movementAudio.Play();
        movementAudio.loop = true;
    }

    private void StopMovementAudio()
    {
        if (movementAudio.isPlaying)
            movementAudio.Stop();
    }

    public void RequestActivation()
    {
        if (currentState == IAState.Imitating)
        {
            AIManager.Instance.TryActivateEnemy(this);
        }
    }

    public void SetFocused(bool focused)
    {
        Debug.Log("Apuntando");
        isFocused = focused;
    }

    public void ResetToInitialState()
    {
        agent.Warp(initialPosition);
        transform.rotation = initialRotation;
        SetState(IAState.Imitating);
    }


}
