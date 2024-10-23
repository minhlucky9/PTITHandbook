using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Maze : MonoBehaviour
{
    public static Maze instance; // Singleton instance

    [Header("Configuration")]
    public int star = 0;
    public TextMeshProUGUI StarScore;
    public Slider StarSlider;
    public GameObject CompleteMazeStarUI;
    public int currentGold { get; private set; }


    [Header("MovementFrezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;
    public MouseManager MouseManager;
    public ButtonActivator Activator;

    private void Update()
    {
        StarScore.text = star + "/2";
        StarSlider.value = star;
    }

    private void Awake()
    {
        // Implement the singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onMazeCollected += GoldGained;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onMazeCollected -= GoldGained;
    }

    private void GoldGained()
    {
        star++;
        if (star == 2)
        {
            CompleteMazeStarUI.SetActive(true);
            MouseManager.ShowCursor();
            Activator.IsUIShow = true;
            if (character.index == 0)
            {
                MovementToggle.isCheck = false;
                CameraToggle.isCheck = false;
            }
            else
            {
                MovementToggle2.isCheck = false;
                CameraToggle2.isCheck = false;
            }
        }
    }

    public int GetStar()
    {
        return star;
    }
}
