using UnityEngine;

public class FlashlightFogController : MonoBehaviour
{
    [SerializeField] private RaycastLight lightController;
    [SerializeField] private ParticleSystemRenderer particleRenderer;

    [Header("Flashlight Fake Fog")]
    [SerializeField] private float flashlightFogAlphaOn = 0.25f;
    [SerializeField] private float flashlightFogFadeSpeed = 4f;

    [Header("World Fog")]
    [SerializeField] private float worldFogDensityWhenLightOff = 0.035f;
    [SerializeField] private float worldFogDensityWhenLightOn = 0.015f;
    [SerializeField] private float worldFogFadeSpeed = 2f;
    [SerializeField] private Color worldFogColorOn = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color worldFogColorOff = new Color(0.08f, 0.08f, 0.08f);

    private Material fogMaterial;
    private float currentFlashlightAlpha;
    private float targetFlashlightAlpha;

    private float currentWorldFogDensity;
    private float targetWorldFogDensity;

    private Color currentWorldFogColor;
    private Color targetWorldFogColor;

    private bool updating;

    private void Awake()
    {
        fogMaterial = particleRenderer.material;
        RenderSettings.fogDensity = worldFogDensityWhenLightOff;
        currentWorldFogDensity = RenderSettings.fogDensity;
        currentWorldFogColor = RenderSettings.fogColor;

        SetFlashlightFogAlpha(0f);
    }

    private void Start()
    {
        lightController.OnLightPowered.AddListener(OnFlashlightOn);
        lightController.OnLightDepleted.AddListener(OnFlashlightOff);
    }

    private void Update()
    {
        if (!updating) return;

        currentFlashlightAlpha = Mathf.MoveTowards(
            currentFlashlightAlpha,
            targetFlashlightAlpha,
            flashlightFogFadeSpeed * Time.deltaTime
        );

        currentWorldFogDensity = Mathf.MoveTowards(
            currentWorldFogDensity,
            targetWorldFogDensity,
            worldFogFadeSpeed * Time.deltaTime
        );

        currentWorldFogColor = Color.Lerp(
            currentWorldFogColor,
            targetWorldFogColor,
            worldFogFadeSpeed * Time.deltaTime
        );

        SetFlashlightFogAlpha(currentFlashlightAlpha);
        RenderSettings.fogDensity = currentWorldFogDensity;
        RenderSettings.fogColor = currentWorldFogColor;

        if (Mathf.Approximately(currentFlashlightAlpha, targetFlashlightAlpha) &&
            Mathf.Approximately(currentWorldFogDensity, targetWorldFogDensity))
        {
            updating = false;
        }
    }

    private void OnFlashlightOn()
    {
        targetFlashlightAlpha = flashlightFogAlphaOn;
        targetWorldFogDensity = worldFogDensityWhenLightOn;
        updating = true;
    }

    private void OnFlashlightOff()
    {
        targetFlashlightAlpha = 0f;
        targetWorldFogDensity = worldFogDensityWhenLightOff;
        targetWorldFogColor = worldFogColorOn;
        updating = true;
    }

    private void SetFlashlightFogAlpha(float alpha)
    {
        Color c = fogMaterial.color;
        c.a = alpha;
        fogMaterial.color = c;
    }
}
