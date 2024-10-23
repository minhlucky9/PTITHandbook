using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFinish : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForJames;
    private bool IsFinish = false;
    public GameObject CompleteUI;
    public QuestPoint QuizMazePoint;

    [Header("MovementFrezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;
    public MouseManager MouseManager;
    public ButtonActivator Activator;


    private void OnTriggerEnter(Collider other)
    {
        if (!IsFinish)
        {
            if (other.CompareTag("Player"))
            {
                if (QuizMazePoint.currentQuestState.Equals(QuestState.CAN_FINISH))
                {
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
                    GameEventsManager.instance.questEvents.FinishQuest(questInfoForJames.id);
                    CompleteUI.SetActive(true);
                    IsFinish = true;
                }
            }
        }
        
    }
}
