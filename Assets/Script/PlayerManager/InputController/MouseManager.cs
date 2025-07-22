using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;
    public GameObject menuUI;
    public GameObject pauseUI;
    void Awake()
    {
        instance = this;
        // Ẩn con trỏ chuột khi bắt đầu game
        Cursor.visible = false;
        // Khóa con trỏ chuột ở giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.M) )
        {
            menuUI.SetActive(true);
            
            ShowCursor();
        }
        */
        
        if (Input.GetKeyDown(KeyCode.Escape) && !PlayerManager.instance.isInteract)
        {
            pauseUI.SetActive(true);
            PlayerManager.instance.DeactivateController();
            PlayerManager.instance.isInteract = true;
            ShowCursor();
        }
        
    }
    public void ShowCursor()
    {
        // Hiện con trỏ chuột
        Cursor.visible = true;
        // Mở khóa con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
    }


    public void HideCursor()
    {
        // Ẩn con trỏ chuột
        Cursor.visible = false;
        // Khóa con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenInteract()
    {
        PlayerManager.instance.isInteract = false;
  
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
