using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
   public GameObject pauseMenuUI;
    public MouseManager mouseManager;
    public ButtonActivator ButtonActivator;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
                mouseManager.HideCursor();
                ButtonActivator.IsUIShow = false;
            }
            else
            {
                ButtonActivator.ISShow();
                Pause();
                mouseManager.ShowCursor();
               
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadGame()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

