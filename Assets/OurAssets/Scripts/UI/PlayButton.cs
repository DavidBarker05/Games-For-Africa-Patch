using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    [SerializeField]
    int nextSceneIndex = 1;

    void Awake() => GetComponent<Button>().onClick.AddListener(async () => await SceneManager.LoadSceneAsync(nextSceneIndex));
}
