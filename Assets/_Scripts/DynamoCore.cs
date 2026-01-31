using UnityEngine;

public class DynamoCore : MonoBehaviour
{
    [SerializeField] private float accelerationPerInput = 150f;
    [SerializeField] private float maxAngularSpeed = 720f;
    [SerializeField] private float friction = 450f;
    [SerializeField] private float energyPerDegree = 0.02f;

    [SerializeField] private DynamoBattery battery;

    private float angularSpeed;
    private float direction = 1f;
    private float inputTimer;

    private bool addedChargeThisFrame;
    private float timeSinceLastCharge;
    private float normalizedCharge;

    public bool _addedChargeThisFrame => addedChargeThisFrame;
    public float _timeSinceLastCharge => timeSinceLastCharge;
    public float _normalizedCharge => normalizedCharge;


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

}
