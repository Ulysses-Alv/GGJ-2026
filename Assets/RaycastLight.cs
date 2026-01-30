using UnityEngine;

public class RaycastLight : MonoBehaviour
{
    public enum RayDirection
    {
        Forward,
        Backward,
        Right,
        Left
    }

    [SerializeField] private DynamoCore dynamoCore;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private string[] targetTags;
    [SerializeField] private RayDirection rayDirection = RayDirection.Forward;

    private RaycastHit[] hits;

    private void Update()
    {
        if (!dynamoCore.IsLightActive())
            return;

        Vector3 direction = GetDirection();

        hits = Physics.RaycastAll(
            transform.position,
            direction,
            rayDistance
        );

        for (int i = 0; i < hits.Length; i++)
        {
            Collider collider = hits[i].collider;

            for (int j = 0; j < targetTags.Length; j++)
            {
                if (collider.CompareTag(targetTags[j]))
                {
                    OnHit(collider, hits[i]);
                }
            }
        }
    }

    protected virtual void OnHit(Collider collider, RaycastHit hit)
    {
        collider.GetComponent<AIEnemy>().SetFocused(true);
    }

    private Vector3 GetDirection()
    {
        if (rayDirection == RayDirection.Forward) return transform.forward;
        if (rayDirection == RayDirection.Backward) return -transform.forward;
        if (rayDirection == RayDirection.Right) return transform.right;
        return -transform.right;
    }

    private void OnDrawGizmos()
    {
        if (!dynamoCore.IsLightActive())
            return;

        Vector3 direction = GetDirection();

        Gizmos.color = Color.red;
        Vector3 origin = transform.position;
        Vector3 end = origin + direction * rayDistance;

        Gizmos.DrawLine(origin, end);
        Gizmos.DrawSphere(end, 0.05f);
    }
}
