[System.Serializable]
public class Dialogue
{
    public string title;
    public string speaker;
    public UnityEngine.Sprite speakerImage;
    [UnityEngine.TextArea]
    public string text;
    [UnityEngine.Min(1f)]
    public float charactersPerSecond;
    [UnityEngine.Min(1f)]
    public float fontSize;
}