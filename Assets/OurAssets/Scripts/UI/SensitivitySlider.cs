using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensitivitySlider : MonoBehaviour
{
    Slider slider;

    void Awake() => slider = GetComponent<Slider>();

    void Start()
    {
        if (UserSettingsManager.Instance != null) slider.value = Mathf.Clamp(UserSettingsManager.Instance.UserSettings.sensitivityMultiplier, slider.minValue, slider.maxValue);
        slider.onValueChanged.AddListener(ChangeSensitivityMultiplier);
    }

    void OnDestroy() => slider.onValueChanged.RemoveListener(ChangeSensitivityMultiplier);

    void ChangeSensitivityMultiplier(float value)
    {
        if (UserSettingsManager.Instance != null) UserSettingsManager.Instance.UserSettings.sensitivityMultiplier = Mathf.Round(value * 100f) / 100f;
    }
}
