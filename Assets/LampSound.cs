using UnityEngine;

public class LampSound : MonoBehaviour
{
    [SerializeField] private AudioClip depleted;
    [SerializeField] private AudioClip powered;
    [SerializeField] private AudioClip charging;

    [SerializeField] private DynamoCore core;
    [SerializeField] private RaycastLight lightSource;
    [SerializeField] private AudioSource audioSource;
    private bool isPlayingCharging;

    void OnEnable()
    {
        core.OnChargeStarted.AddListener(HandleChargeStarted);
        core.OnChargeStopped.AddListener(HandleChargeStopped);

        lightSource.OnLightPowered.AddListener(HandleLightPowered);
        lightSource.OnLightDepleted.AddListener(HandleLightDepleted);
    }

    void OnDisable()
    {
        core.OnChargeStarted.RemoveListener(HandleChargeStarted);
        core.OnChargeStopped.RemoveListener(HandleChargeStopped);

        lightSource.OnLightPowered.RemoveListener(HandleLightPowered);
        lightSource.OnLightDepleted.RemoveListener(HandleLightDepleted);
    }

    private void HandleChargeStarted()
    {
        audioSource.clip = charging;
        audioSource.loop = true;
        audioSource.Play();
        isPlayingCharging = true;
    }
    private void Update()
    {
        if (isPlayingCharging)
        {
            var charge = core.NormalizedCharge;
            audioSource.pitch = Mathf.Lerp(1f, 0.75f, charge);
        }
    }

    private void HandleChargeStopped()
    {
        audioSource.Stop(); isPlayingCharging = false;
    }

    private void HandleLightPowered()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(powered);
        isPlayingCharging = false;
    }

    private void HandleLightDepleted()
    {
        audioSource.PlayOneShot(depleted);
        isPlayingCharging = false;
    }
}
