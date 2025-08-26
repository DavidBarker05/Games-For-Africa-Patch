using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleOnScreenStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    float movementRange = 50f;

    RectTransform handle;
    Vector2 startPos;
    Vector2 input;
    bool isPressed = false;

    public Vector2 Input => input;

    void Awake()
    {
        handle = GetComponent<RectTransform>();
        if (handle != null) startPos = handle.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData) => isPressed = true;

    public void OnDrag(PointerEventData eventData)
    {
        if (handle == null && !isPressed) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)handle.parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        Vector2 delta = Vector2.ClampMagnitude(localPoint - startPos, movementRange);
        handle.anchoredPosition = startPos + delta;
        Vector2 normalizedInput = new Vector2(Mathf.Clamp(delta.x / movementRange, -1f, 1f), Mathf.Clamp(delta.y / movementRange, -1f, 1f));
        input = Vector2.ClampMagnitude(normalizedInput, 1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (handle != null) handle.anchoredPosition = startPos;
        input = Vector2.zero;
    }
}
