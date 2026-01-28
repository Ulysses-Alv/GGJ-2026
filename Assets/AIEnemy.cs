using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float blindRecoveryTime = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isActive = true;
    private bool isBlinded = false;
    private float blindTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        AIManager.Instance.SubscribeEnemy(this);
    }

    void Update()
    {
        if (isActive && !isBlinded && player != null)
        {
            agent.SetDestination(player.position);
        }

        if (isBlinded)
        {
            blindTimer += Time.deltaTime;
            if (blindTimer >= blindRecoveryTime)
            {
                isBlinded = false;
                agent.isStopped = !isActive;
            }
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        agent.isStopped = !active;
    }

    public void Blind()
    {
        isBlinded = true;
        agent.isStopped = true;
        blindTimer = 0f;
    }
}
