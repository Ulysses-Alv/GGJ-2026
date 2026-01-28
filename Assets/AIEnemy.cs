using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    bool isActive = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        AIManager.Instance.SubscribeEnemy(this);
    }

    void Update()
    {
        if (isActive && player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        agent.isStopped = !active;
    }

    public void Blind()
    {
        agent.isStopped = true;
    }
}
