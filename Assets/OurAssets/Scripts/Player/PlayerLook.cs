using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputScript))] // Ensure attached to player
public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField, Min(0f)]
    float mouseHorizontalSensitivity = 0.2f;
    [SerializeField, Min(0f)]
    float mouseVerticalSensitivity = 0.225f;
    [SerializeField, Min(0f)]
    float controllerHorizontalSensitivity = 180f;
    [SerializeField, Min(0f)]
    float controllerVerticalSensitivity = 202.5f;
    [SerializeField, Min(0f)]
    float onScreenHorizontalSensitivity = 3f;
    [SerializeField, Min(0f)]
    float onScreenVerticalSensitivity = 3f;
    [SerializeField, Range(-90f, 0f)]
    float minVerticalAngle = -80f;
    [SerializeField, Range(0f, 90f)]
    float maxVerticalAngle = 80f;

    float pitch = 0f;
    PlayerInputScript pIS;
    bool isLocked = false;

    void Awake()
    {
        pIS = GetComponent<PlayerInputScript>();
        #if UNITY_EDITOR
            pIS.AddMouseLockUnlockAction(LockOrUnlock);
        #endif
        #if UNITY_STANDALONE
            DisableCursor();
        #endif
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        float mX = pIS.LookInput.x;
        float mY = pIS.LookInput.y;
        float hSens = (pIS.LookDevice is Mouse) ? mouseHorizontalSensitivity : (((pIS.LookDevice is Gamepad) ? controllerHorizontalSensitivity : onScreenHorizontalSensitivity) * Time.deltaTime);
        float vSens = (pIS.LookDevice is Mouse) ? mouseVerticalSensitivity : (((pIS.LookDevice is Gamepad) ? controllerVerticalSensitivity : onScreenVerticalSensitivity) * Time.deltaTime);
        float yaw = mX * hSens * (UserSettingsManager.Instance?.UserSettings.sensitivityMultiplier ?? 1);
        pitch -= mY * vSens * (UserSettingsManager.Instance?.UserSettings.sensitivityMultiplier ?? 1);
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
        if (cam != null) cam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up, yaw);
    }

    void OnDestroy()
    {
        #if UNITY_EDITOR
            pIS.RemoveMouseLockUnlockAction(LockOrUnlock);
        #endif
    }

    void LockOrUnlock(InputAction.CallbackContext obj)
    {
        if (isLocked) EnableCursor();
        else DisableCursor();
        isLocked = !isLocked;
    }

    public void DisableCursor()
    {
        if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
        if (Cursor.visible) Cursor.visible = false;
    }

    public void EnableCursor()
    {
        if (Cursor.lockState != CursorLockMode.Confined) Cursor.lockState = CursorLockMode.Confined;
        if (!Cursor.visible) Cursor.visible = true;
    }
}
