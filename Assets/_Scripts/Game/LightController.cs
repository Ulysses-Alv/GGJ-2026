using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController instance;

    [SerializeField] private Light[] lights;
    [SerializeField] private AudioSource audioS;

    private void Awake()
    {
        instance = this;
    }
    public void SwitchLights(bool active)
    {
        foreach (var item in lights)
        {
            item.enabled = active;

        }
        if (!active)
        {
            audioS.Play();
        }

    }
}
