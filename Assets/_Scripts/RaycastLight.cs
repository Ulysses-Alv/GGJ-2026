using UnityEngine;
using UnityEngine.Events;

public class RaycastLight : MonoBehaviour
{
    public UnityEvent OnLightPowered;
    public UnityEvent OnLightDepleted;

    [SerializeField] private DynamoCore dynamoCore;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private string[] targetTags;

    [SerializeField] private float maxIntensity = 250f;
    [SerializeField] private Light lightSource;

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float idleTimeToEnableLight = 0.35f;
    [SerializeField] private float fullChargeLockTime = 2f;

    private bool isLightActive;
    private bool fullChargeConsumed;

    public bool IsLightActive => isLightActive;

    private void Update()
    {
        bool reachedFullCharge = dynamoCore.NormalizedCharge >= 1f;

        if (reachedFullCharge && !fullChargeConsumed)
        {
            fullChargeConsumed = true;
            dynamoCore.LockCharge(fullChargeLockTime);
        }

        bool canEnableLight =
            reachedFullCharge ||
            (
                dynamoCore.NormalizedCharge > 0.4f &&
                (
                    dynamoCore.TimeSinceLastCharge >= idleTimeToEnableLight ||
                    (dynamoCore.NormalizedCharge >= 0.9f && !dynamoCore.AddedChargeThisFrame)
                )
            );

        if (canEnableLight)
        {
            float curveValue = curve.Evaluate(dynamoCore.NormalizedCharge);
            lightSource.intensity = curveValue * maxIntensity;
        }
        else
        {
            lightSource.intensity = 0f;
            fullChargeConsumed = false;
        }

        UpdateLightState();

        if (!isLightActive)
            return;

        Raycast();
    }

    private void UpdateLightState()
    {
        bool hasLight = lightSource.intensity > 0f;

        if (!isLightActive && hasLight)
        {
            isLightActive = true;
            OnLightPowered?.Invoke();
        }

        if (isLightActive && !hasLight)
        {
            isLightActive = false;
            OnLightDepleted?.Invoke();
        }
    }

    private void Raycast()
    {
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.RaycastAll(
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
                    collider.GetComponent<AIEnemy>()?.SetFocused(true);
                }
            }
        }
    }
}
