using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class UserSettingsManager : MonoBehaviour
{
    public static UserSettingsManager Instance { get; private set; }

    [SerializeField]
    AudioMixer audioMixer;

    string path = "";
    UserSettings previousSettings;

    public UserSettings UserSettings { get; set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        path = Path.Combine(Application.persistentDataPath, "user_settings.json");
        if (!File.Exists(path))
        {
            UserSettings = new UserSettings();
            SaveSettings();
        }
        else LoadSettings();
        previousSettings = new UserSettings(UserSettings);
    }

    void Start()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(UserSettings.musicVolume, 0.0001f, 1f)) * 20f);
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Clamp(UserSettings.effectsVolume, 0.0001f, 1f)) * 20f);
    }

    void Update()
    {
        if (previousSettings.sensitivityMultiplier != UserSettings.sensitivityMultiplier) previousSettings.sensitivityMultiplier = UserSettings.sensitivityMultiplier;
        if (previousSettings.musicVolume != UserSettings.musicVolume)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(UserSettings.musicVolume, 0.0001f, 1f)) * 20f);
            previousSettings.musicVolume = UserSettings.musicVolume;
        }
        if (previousSettings.effectsVolume != UserSettings.effectsVolume)
        {
            audioMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Clamp(UserSettings.effectsVolume, 0.0001f, 1f)) * 20f);
            previousSettings.effectsVolume = UserSettings.effectsVolume;
        }
    }

    void OnApplicationQuit() => SaveSettings();

    void OnApplicationPause(bool pause)
    {
        if (pause) SaveSettings();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(UserSettings, prettyPrint: true);
        File.WriteAllText(path, json);
    }

    void LoadSettings()
    {
        string json = File.ReadAllText(path);
        UserSettings = JsonUtility.FromJson<UserSettings>(json);
    }
}