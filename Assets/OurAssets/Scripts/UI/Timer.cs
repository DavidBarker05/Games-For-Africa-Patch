using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float totalTime = 60f; 
    public TextMeshProUGUI timerText;
    private float timeRemaining;

    [SerializeField] GameObject DeathMenu;
    [SerializeField] PlayerLook PlayerLook;
 
    void Start()
    {
        Time.timeScale = 1f;
        timeRemaining = totalTime; 
        DeathMenu.SetActive(false);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; 
            UpdateTimerDisplay();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerDisplay();
            Debug.Log("Timer ended!");
            DeathMenu.SetActive(true);
            Time.timeScale = 0f;
            PointsController.instance.DisplayPoints();
            PlayerLook.EnableCursor();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); 
        int seconds = Mathf.FloorToInt(timeRemaining % 60); 
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
    }

    public void PuaseTimer()
    {
        Time.timeScale = 0f;
    }

    public void UnPauseTimer()
    {
        Time.timeScale = 1.0f;
    }
}
