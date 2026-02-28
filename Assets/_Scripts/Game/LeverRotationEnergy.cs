using System;
using UnityEngine;

public class LeverRotationEnergy : MonoBehaviour
{
    [SerializeField] private DynamoCore dynamo;
    [SerializeField] private Transform leverTransform;
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float energyMultiplier;

    private float lastAngle;

    private void Awake()
    {
        lastAngle = GetAngle();
    }

    private void Update()
    {
        float currentAngle = GetAngle();
        float delta = Mathf.Abs(currentAngle - lastAngle);
        lastAngle = currentAngle;

        if (delta > 0f)
            dynamo.AddImpulse(delta > 0f);
    }

    private float GetAngle()
    {
        return Vector3.Dot(leverTransform.localEulerAngles, rotationAxis);
    }
}
