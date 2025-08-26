using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    [SerializeField]
    SimpleOnScreenStick rightScreenStick;
    [Header("Actions")]
    [SerializeField]
    InputActionReference moveAction;
    [SerializeField]
    InputActionReference lookAction;
    [SerializeField]
    InputActionReference jumpAction;
    [SerializeField]
    InputActionReference interactAction;
    [SerializeField]
    InputActionReference mouseLockUnlockAction;
    [SerializeField]
    InputActionReference pauseAction;

    public Vector2 MoveInput => moveAction.action.ReadValue<Vector2>();
    public Vector2 LookInput
    {
        get
        {
            if (rightScreenStick != null && rightScreenStick.Input.sqrMagnitude > 0f) return rightScreenStick.Input;
            if (LookDevice == null) return Vector2.zero;
            if (LookDevice is Touchscreen) return Vector2.zero;
            return lookAction.action.ReadValue<Vector2>();
        }
    }
    public InputDevice LookDevice { get; private set; }
    public void AddJumpAction(System.Action<InputAction.CallbackContext> action) => jumpAction.action.started += action;
    public void RemoveJumpAction(System.Action<InputAction.CallbackContext> action) => jumpAction.action.started -= action;
    public void AddInteractAction(System.Action<InputAction.CallbackContext> action) => interactAction.action.started += action;
    public void RemoveInteractAction(System.Action<InputAction.CallbackContext> action) => interactAction.action.started -= action;
    public void AddMouseLockUnlockAction(System.Action<InputAction.CallbackContext> action) => mouseLockUnlockAction.action.started += action;
    public void RemoveMouseLockUnlockAction(System.Action<InputAction.CallbackContext> action) => mouseLockUnlockAction.action.started -= action;
    public void AddPauseAction(System.Action<InputAction.CallbackContext> action) => pauseAction.action.started += action;
    public void RemovePauseAction(System.Action<InputAction.CallbackContext> action) => pauseAction.action.started -= action;

    void Awake()
    {
        lookAction.action.performed += DetectLookInput;
        lookAction.action.Enable();
    }

    void OnDestroy() => lookAction.action.performed -= DetectLookInput;

    void DetectLookInput(InputAction.CallbackContext ctx) => LookDevice = ctx.control.device;
}