using UnityEngine;

public class DynamoBattery : MonoBehaviour
{
    [SerializeField] private float maxCharge = 100f;
    [SerializeField] private float decayPerSecond = 5f;
    [SerializeField] private Light _light;
    [SerializeField] private float currentCharge;

    [SerializeField] private float maxIntensity = 250f;

    void Update()
    {
        currentCharge = Mathf.Max(
            currentCharge - decayPerSecond * Time.deltaTime,
            0f
        );
        _light.intensity = Mathf.Lerp(0, maxIntensity, GetNormalizedCharge());
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
