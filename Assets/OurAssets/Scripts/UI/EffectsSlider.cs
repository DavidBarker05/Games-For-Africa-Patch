using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class EffectsSlider : MonoBehaviour
{
    Slider slider;

    void Awake() => slider = GetComponent<Slider>();

    void Start()
    {
        if (UserSettingsManager.Instance != null) slider.value = Mathf.Clamp(UserSettingsManager.Instance.UserSettings.effectsVolume, slider.minValue, slider.maxValue);
        slider.onValueChanged.AddListener(ChangeEffectsVolume);
    }

    void OnDestroy() => slider.onValueChanged.RemoveListener(ChangeEffectsVolume);

    void ChangeEffectsVolume(float value)
    {
        if (UserSettingsManager.Instance != null) UserSettingsManager.Instance.UserSettings.effectsVolume = Mathf.Round(value * 10000f) / 10000f;
    }
}
