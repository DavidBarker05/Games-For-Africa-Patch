[System.Serializable]
public class UserSettings
{
    public float sensitivityMultiplier = 1f;
    public float musicVolume = 1f;
    public float effectsVolume = 1f;

    /// <summary>
    /// <para>Default constructor</para>
    /// <list type="bullet">
    /// <item>Sensitivity Multiplier = 1f</item>
    /// <item>Music Volume = 1f</item>
    /// <item>Effects Volume = 1f</item>
    /// </list>
    /// </summary>
    public UserSettings()
    {
        sensitivityMultiplier = 1f;
        musicVolume = 1f;
        effectsVolume = 1f;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="other">The UserSettings object to copy values from</param>
    public UserSettings(UserSettings other)
    {
        sensitivityMultiplier = other.sensitivityMultiplier;
        musicVolume = other.musicVolume;
        effectsVolume = other.effectsVolume;
    }
}