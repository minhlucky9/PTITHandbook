using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public static PauseMenuUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Resume()
    {
      
    
        GameIsPaused = false;
    }

    public void Pause()
    {
   
        GameIsPaused = true;
    }

    public void PauseGlobal()
    {

        Time.timeScale = 0f;
        //  GameIsPaused = true;
    }

    public void ResumeGlobal()
    {

       
        GameIsPaused = false;
    }


    public void LoadMenu()
    {
        SceneManager.LoadScene("UI");
    }
}
