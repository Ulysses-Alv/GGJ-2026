using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensibilitySlider : MonoBehaviour
{
    private Slider slider;

    [SerializeField] private float defaultValue = 0.5f;
    [SerializeField] private TextMeshProUGUI textNum;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    void Start()
    {
        slider.value = defaultValue;
        Sensi.sens = defaultValue;
        textNum.text = defaultValue.ToString("0.00");
        slider.onValueChanged.AddListener(OnValueChange);
    }

    private void OnValueChange(float arg0)
    {
        Sensi.sens = arg0;
        textNum.text = arg0.ToString("0.00");
    }
}

public static class Sensi
{
    public static float sens = 0.5f;
}