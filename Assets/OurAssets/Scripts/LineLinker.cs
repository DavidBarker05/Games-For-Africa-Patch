using UnityEngine;
using System.Collections.Generic; // For List<Transform>

public class LineLinker : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Transform> linkedObjects; // Or public Transform[] linkedObjects;

    void Start()
    {
        // Ensure lineRenderer is assigned (e.g., if you dragged it in the Inspector)
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Set the number of positions for the Line Renderer
        if (linkedObjects != null)
        {
            lineRenderer.positionCount = linkedObjects.Count;
        }
    }

    void Update()
    {
        // Update the Line Renderer's positions every frame
        if (linkedObjects != null && linkedObjects.Count > 0)
        {
            for (int i = 0; i < linkedObjects.Count; i++)
            {
                if (linkedObjects[i] != null)
                {
                    lineRenderer.SetPosition(i, linkedObjects[i].position);
                }
            }
        }
    }
}