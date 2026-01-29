using UnityEngine;

public class DynamoCore : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private Axis axis = Axis.Z;
    [SerializeField] private float accelerationPerInput = 150f;
    [SerializeField] private float maxAngularSpeed = 720f;
    [SerializeField] private float friction = 450f;
    [SerializeField] private float energyPerDegree = 0.02f;

    [SerializeField] private DynamoBattery battery;

    [SerializeField] private float maxIntensity = 250f;

    [SerializeField] private Light _light;

    private float angularSpeed;
    private float direction = 1f;
    private float inputTimer;

    [SerializeField] private AnimationCurve curve;
    private float normalizedCharge;

    void Update()
    {
        inputTimer -= Time.deltaTime;

        if (inputTimer <= 0f)
        {
            angularSpeed = Mathf.MoveTowards(
                angularSpeed,
                0f,
                friction * Time.deltaTime
            );
        }

        if (angularSpeed != 0f)
        {
            float deltaRotation = angularSpeed * direction * Time.deltaTime;
            float energy = Mathf.Abs(deltaRotation) * energyPerDegree;

            battery.AddCharge(energy);
        }

        normalizedCharge = battery.GetNormalizedCharge();
        float curveValue = curve.Evaluate(normalizedCharge);

        _light.intensity = curveValue * maxIntensity;
    }

    public void AddImpulse(bool isRight = true)
    {
        direction = isRight ? 1f : -1f;

        angularSpeed = Mathf.Clamp(
            angularSpeed + accelerationPerInput,
            0f,
            maxAngularSpeed
        );

        inputTimer = 0.12f;
    }

    public float GetNormalizedSpeed()
    {
        return angularSpeed / maxAngularSpeed;
    }
    public bool isActiveLight()
    {
        return normalizedCharge > 0.4f;
    }
}


public class RaycastLight : MonoBehaviour
{
    [SerializeField] private DynamoCore dynamoCore;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private string[] targetTags;

    private RaycastHit[] hits;

    private void Update()
    {
        if (!dynamoCore.isActiveLight()) return;

        hits = Physics.RaycastAll(transform.position, transform.forward, rayDistance);

        foreach (var hit in hits)
        {
            for (int i = 0; i < targetTags.Length; i++)
            {
                if (hit.collider.CompareTag(targetTags[i]))
                {

                }
            }
        }
    }
}