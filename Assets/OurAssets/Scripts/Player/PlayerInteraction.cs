using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputScript))] // Ensure attached to player
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField, Min(0f)]
    float maxInteractionDistance = 2f;
    [SerializeField, Range(0.1f, 0.5f)]
    float interactionRadius;
    [SerializeField]
    LayerMask interactableLayer;
    [SerializeField]
    Transform holdPos;
    [SerializeField]
    LayerMask holdLayer;
    [SerializeField]
    Camera holdCamera;
    [SerializeField]
    Camera holdClipCamera;

    Interactable currentInteraction;
    PlayerInputScript pIS;

    public bool IsHolding { get; private set; }

    void Awake()
    {
        pIS = GetComponent<PlayerInputScript>();
        pIS.AddInteractAction(Interact);
    }

    void Update()
    {
        if (currentInteraction == null) return;
        if (currentInteraction is Holdable heldObject)
        {
            heldObject.LookAtPlayer(transform.position);
            int bitMask = ~(holdLayer | gameObject.layer);
            if (Physics.Linecast(cam.transform.position, heldObject.transform.position, bitMask) || Physics.CheckBox(heldObject.transform.position, heldObject.GetComponent<Collider>().bounds.extents, heldObject.transform.rotation, bitMask))
            {
                holdCamera.gameObject.SetActive(false);
                holdClipCamera.gameObject.SetActive(true);
            }
            else
            {
                holdCamera.gameObject.SetActive(true);
                holdClipCamera.gameObject.SetActive(false);
            }
        }
    }

    void OnDestroy() => pIS.RemoveInteractAction(Interact);

    void Interact(InputAction.CallbackContext obj)
    {
        Interactable targetInteraction = null;
        if (Physics.SphereCast(cam.transform.position, interactionRadius, cam.transform.forward, out RaycastHit interactHit, maxInteractionDistance, interactableLayer)) targetInteraction = interactHit.collider.GetComponent<Interactable>();

        if (targetInteraction != null)
        {
            if (currentInteraction != null)
            {
                if (currentInteraction is Holdable) InteractHoldable();
            }
            else
            {
                currentInteraction = targetInteraction;
                if (currentInteraction is Holdable) InteractHoldable();
                else if (currentInteraction.Interact()) currentInteraction = null;
            }
        }
        else if (currentInteraction != null)
        {
            if (currentInteraction is Holdable) InteractHoldable();
        }
    }

    void InteractHoldable()
    {
        if (currentInteraction is Holdable holdable) // Extra safety measure
        {
            bool drop = holdable.Interact(holdPos, holdLayer, GetComponent<Collider>(), cam);
            IsHolding = !drop;
            if (drop) currentInteraction = null;
        }
    }
}