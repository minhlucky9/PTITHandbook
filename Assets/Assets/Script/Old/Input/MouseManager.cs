using UnityEngine;

public class MouseManager : MonoBehaviour
{
    void Awake()
    {
        // ?n con tr? chu?t khi b?t ??u game
        Cursor.visible = false;
        // Khóa con tr? chu?t ? gi?a màn hình
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Hàm ?? hi?n th? l?i con tr? chu?t khi c?n
    public void ShowCursor()
    {
        // Hi?n th? con tr? chu?t
        Cursor.visible = true;
        // M? khóa con tr? chu?t
        Cursor.lockState = CursorLockMode.None;
    }

    // Hàm ?? ?n l?i con tr? chu?t
    public void HideCursor()
    {
        // ?n con tr? chu?t
        Cursor.visible = false;
        // Khóa con tr? chu?t
        Cursor.lockState = CursorLockMode.Locked;
    }
}
