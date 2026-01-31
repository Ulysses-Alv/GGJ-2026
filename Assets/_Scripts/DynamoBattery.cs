using UnityEngine;

public class DynamoBattery : MonoBehaviour
{
    [SerializeField] private float maxCharge = 100f;
    [SerializeField] private float decayPerSecond = 5f;

    [SerializeField] private float currentCharge;

    void Update()
    {
        currentCharge = Mathf.Max(
            currentCharge - decayPerSecond * Time.deltaTime,
            0f
        );
    }

    public void AddCharge(float amount)
    {
        currentCharge = Mathf.Clamp(
            currentCharge + amount,
            0f,
            maxCharge
        );
    }

    public float GetNormalizedCharge()
    {
        return currentCharge / maxCharge;
    }
}
