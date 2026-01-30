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
    [SerializeField] private Light lightSource;

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float idleTimeToEnableLight = 0.35f;

    private float angularSpeed;
    private float direction = 1f;
    private float inputTimer;
    private float normalizedCharge;

    private float timeSinceLastCharge;
    private bool addedChargeThisFrame;

    void Update()
    {
        addedChargeThisFrame = false;
        inputTimer -= Time.deltaTime;
        timeSinceLastCharge += Time.deltaTime;

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

            if (energy > 0f)
            {
                battery.AddCharge(energy);
                addedChargeThisFrame = true;
                timeSinceLastCharge = 0f;
            }
        }

        normalizedCharge = battery.GetNormalizedCharge();

        bool canEnableLight =
            normalizedCharge > 0.4f &&
            (
                timeSinceLastCharge >= idleTimeToEnableLight ||
                (normalizedCharge >= 0.9f && !addedChargeThisFrame)
            );

        if (canEnableLight)
        {
            float curveValue = curve.Evaluate(normalizedCharge);
            lightSource.intensity = curveValue * maxIntensity;
        }
        else
        {
            lightSource.intensity = 0f;
        }
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

    public bool IsLightActive()
    {
        return lightSource.intensity > 0f;
    }
}
