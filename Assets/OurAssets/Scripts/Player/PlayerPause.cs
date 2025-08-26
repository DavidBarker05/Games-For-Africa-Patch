using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

[RequireComponent(typeof(PlayerInputScript), typeof(PlayerLook))]
public class PlayerPause : MonoBehaviour
{
    [SerializeField]
    GameObject pauseScreen;
    [SerializeField]
    AudioManager audioManager;

    PlayerInputScript pIS;
    PlayerLook pL;
    bool isPaused = false;

    void Awake()
    {
        pIS = GetComponent<PlayerInputScript>();
        pL = GetComponent<PlayerLook>();
        pIS.AddPauseAction(PauseOrUnpause);
    }

    void OnDisable() => UndoChanges();

    void OnDestroy()
    {
        pIS.RemovePauseAction(PauseOrUnpause);
        UndoChanges();
    }

    void PauseOrUnpause(InputAction.CallbackContext ctx)
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pauseScreen != null) pauseScreen.SetActive(isPaused);
        if (isPaused) pL.EnableCursor();
        else pL.DisableCursor();
        if (audioManager != null) audioManager.PlayButtons();
    }

    void UndoChanges()
    {
        if (Time.timeScale == 0f) Time.timeScale = 1f;
        pL.DisableCursor();
    }
}
