using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MusicSlider : MonoBehaviour
{
    Slider slider;

    void Awake() => slider = GetComponent<Slider>();

    void Start()
    {
        if (UserSettingsManager.Instance != null) slider.value = Mathf.Clamp(UserSettingsManager.Instance.UserSettings.musicVolume, slider.minValue, slider.maxValue);
        slider.onValueChanged.AddListener(ChangeMusicVolume);
    }

    void OnDestroy() => slider.onValueChanged.RemoveListener(ChangeMusicVolume);

    void ChangeMusicVolume(float value)
    {
        if (UserSettingsManager.Instance != null) UserSettingsManager.Instance.UserSettings.musicVolume = Mathf.Round(value * 10000f) / 10000f;
    }
}
