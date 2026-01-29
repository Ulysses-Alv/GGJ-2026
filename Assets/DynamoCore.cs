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

    private float angularSpeed;
    private float direction = 1f;
    private float inputTimer;

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

        if (angularSpeed == 0f) return;

        float deltaRotation = angularSpeed * direction * Time.deltaTime;
        float energy = Mathf.Abs(deltaRotation) * energyPerDegree;

        battery.AddCharge(energy);

       // Rotate(deltaRotation);
    }

    private void Rotate(float delta)
    {
        Vector3 rotation = Vector3.zero;

        if (axis == Axis.X) rotation.x = delta;
        if (axis == Axis.Y) rotation.y = delta;
        if (axis == Axis.Z) rotation.z = delta;

        transform.Rotate(rotation, Space.Self);
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
