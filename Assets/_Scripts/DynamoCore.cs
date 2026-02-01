using UnityEngine;
using UnityEngine.Events;

public class DynamoCore : MonoBehaviour
{
    [SerializeField] private float accelerationPerInput = 150f;
    [SerializeField] private float maxAngularSpeed = 720f;
    [SerializeField] private float friction = 450f;
    [SerializeField] private float energyPerDegree = 0.02f;
    [SerializeField] private float stopChargeDelay = 0.15f;
    [SerializeField] private DynamoBattery battery;

    public UnityEvent OnChargeStarted;
    public UnityEvent OnChargeStopped;


    private float angularSpeed;
    private float direction = 1f;
    private float inputTimer;

    private bool addedChargeThisFrame;
    private float timeSinceLastCharge;
    private float normalizedCharge;

    private bool isCharging;

    public bool AddedChargeThisFrame => addedChargeThisFrame;
    public float TimeSinceLastCharge => timeSinceLastCharge;
    public float NormalizedCharge => normalizedCharge;
    public bool IsCharging => isCharging;
    [SerializeField] private float chargeLockTime;
    private float chargeLockTimer;

    public bool IsChargeLocked => chargeLockTimer > 0f;

    public void LockCharge(float duration)
    {
        chargeLockTime = duration;
        chargeLockTimer = duration;
    }

    void Update()
    {
        addedChargeThisFrame = false;
        inputTimer -= Time.deltaTime;
        timeSinceLastCharge += Time.deltaTime;
        chargeLockTimer -= Time.deltaTime;

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
            if (energy > 0f && chargeLockTimer <= 0f)
            {
                battery.AddCharge(energy);
                addedChargeThisFrame = true;
                timeSinceLastCharge = 0f;
            }
        }

        UpdateChargeState();

        normalizedCharge = battery.GetNormalizedCharge();
    }

    private void UpdateChargeState()
    {
        if (!isCharging && addedChargeThisFrame)
        {
            isCharging = true;
            OnChargeStarted?.Invoke();
        }

        if (isCharging && timeSinceLastCharge > stopChargeDelay)
        {
            isCharging = false;
            OnChargeStopped?.Invoke();
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
}
