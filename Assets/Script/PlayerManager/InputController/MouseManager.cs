using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    void Awake()
    {
        // Ẩn con trỏ chuột khi bắt đầu game
        Cursor.visible = false;
        // Khóa con trỏ chuột ở giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Hàm ?? hi?n th? l?i con tr? chu?t khi c?n
    public void ShowCursor()
    {
        // Hiện con trỏ chuột
        Cursor.visible = true;
        // Mở khóa con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
    }

    // Hàm ?? ?n l?i con tr? chu?t
    public void HideCursor()
    {
        // Ẩn con trỏ chuột
        Cursor.visible = false;
        // Khóa con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
    }
}
