using UnityEngine;

public class EnableCursorScreen : MonoBehaviour
{
    void Start()
    {
        if (Cursor.lockState != CursorLockMode.Confined) Cursor.lockState = CursorLockMode.Confined;
        if (!Cursor.visible) Cursor.visible = true;
    }
}
