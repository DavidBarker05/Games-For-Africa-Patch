using TMPro;
using UnityEngine;

public class PointsController : MonoBehaviour
{
    public static PointsController instance;

    [SerializeField] TextMeshProUGUI txtWin;
    [SerializeField] TextMeshProUGUI txtLose;

    [SerializeField] int points;

    private void Awake()
    {
        //singleton code
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addPoints()
    {
        points++;
    }

    public void ResetPoints()
    {
        points = 0;
    }

    public void addCertainPoints(int p)
    {
        points = points + p;
    }
    public void DisplayPoints()
    {
        txtWin.text = "Score: " + points;
        txtLose.text = "Score: " + points;
    }
}

