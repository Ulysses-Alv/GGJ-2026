using UnityEngine;

public class HandlePanelSounds : MonoBehaviour
{
    [SerializeField] private AudioClip charging;

    [SerializeField] private DynamoCore core;
    [SerializeField] private AudioSource audioSource;
    private bool isPlayingCharging;

    void OnEnable()
    {
        core.OnChargeStarted.AddListener(HandleChargeStarted);
        core.OnChargeStopped.AddListener(HandleChargeStopped);
    }

    void OnDisable()
    {
        core.OnChargeStarted.RemoveListener(HandleChargeStarted);
        core.OnChargeStopped.RemoveListener(HandleChargeStopped);
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
}

