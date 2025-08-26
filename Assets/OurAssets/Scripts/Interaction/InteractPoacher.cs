using UnityEngine;

public class InteractPoacher : Interactable
{
    [SerializeField]
    private GameObject WinMenu; // The GameObject to show/hide
    [SerializeField]
    private PlayerLook PlayerLook;

    void Start()
    {
        WinMenu.SetActive(false); // Ensure the object is initially hidden
        Time.timeScale = 1.0f;
    }

    public override bool Interact(params object[] parameters)
    {
        if (parameters.Length != 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"WARNING: RevealObject requires 0 parameters. Received {parameters.Length} parameters");
#endif
            return true; // Interaction failed
        }
        else
        {
            if (WinMenu != null)
            {
                PointsController.instance.addCertainPoints(10);
                PointsController.instance.DisplayPoints();
                WinMenu.SetActive(true); // Reveal the object
                Time.timeScale = 0f;
                PlayerLook.EnableCursor();
                return true; // Interaction successful
            }
            else
            {
                Debug.LogWarning("Object to reveal is not assigned in " + gameObject.name);
                return true; // Interaction failed
            }
        }
    }
}
