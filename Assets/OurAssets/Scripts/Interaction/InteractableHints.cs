using UnityEngine;

public class InteractableHints : Interactable
{
    [SerializeField]
    private GameObject objectToReveal; // The GameObject to show/hide
    [SerializeField]
    private float revealDuration = 30f; // Duration the object stays visible

    void Start()
    {
        if (objectToReveal == null)
        {
            Debug.LogWarning("Object to reveal is not assigned in " + gameObject.name);
            return;
        }

        objectToReveal.SetActive(false); // Ensure the object is initially hidden
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
            if (objectToReveal != null)
            {
                objectToReveal.SetActive(true); // Reveal the object
                CancelInvoke("HideObject"); // Cancel any existing hide schedule
                Invoke("HideObject", revealDuration); // Schedule hiding after duration
                return true; // Interaction successful
            }
            else
            {
                Debug.LogWarning("Object to reveal is not assigned in " + gameObject.name);
                return true; // Interaction failed
            }
        }
    }

    private void HideObject()
    {
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(false); // Hide the object
        }
    }
}
