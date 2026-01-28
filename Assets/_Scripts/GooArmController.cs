using UnityEngine;

public class GooArmAttractionController : MonoBehaviour
{
    public string targetTag = "Attractable";
    public float detectionRadius = 2f;
    public float releaseRadius = 3f;

    [SerializeField] private float distanceRef;

    private Material materialInstance;
    private Transform currentTarget;
    private Vector3 initScale;

    void Awake()
    {
        materialInstance = GetComponent<Renderer>().material;
        initScale = transform.localScale;
    }

    void Update()
    {
        UpdateTarget();
        UpdateScaleAndRotation();
        SendTargetToShader();
    }

    void UpdateTarget()
    {
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            if (dist > releaseRadius)
            {
                currentTarget = null;
            }
        }

        if (currentTarget != null)
            return;

        GameObject[] candidates = GameObject.FindGameObjectsWithTag(targetTag);

        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        for (int i = 0; i < candidates.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, candidates[i].transform.position);

            if (dist > detectionRadius)
                continue;

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTarget = candidates[i].transform;
            }
        }

        currentTarget = closestTarget;
    }

    void UpdateScaleAndRotation()
    {
        if (currentTarget == null)
        {
            transform.localScale = initScale;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance < distanceRef)
        {
            float t = Mathf.Clamp01(distance / distanceRef);
            transform.localScale = initScale * t;
        }
        else
        {
            transform.localScale = initScale;
        }

        Vector3 dir = (currentTarget.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(-transform.right, dir) * transform.rotation;
    }

    void SendTargetToShader()
    {
        materialInstance.SetVector("_TargetPosition0", currentTarget ? currentTarget.position : Vector4.zero);
    }
}
