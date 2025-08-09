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
    public UIAnimationController QuestUI;
    public UIAnimationController LeaderBoardUI;
    

    public enum MousePermission
    {
        All,
        OnlyQuest,
        OnlyLeaderBoard,
        Inventory,
        EnterPress,
        None
    }

    public MousePermission permission = MousePermission.All;

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
        if (permission != MousePermission.None)
        {
            if (permission == MousePermission.All && Input.GetKeyDown(KeyCode.Escape) && !PlayerManager.instance.isInteract)
            {
                pauseUI.SetActive(true);
                PlayerManager.instance.DeactivateController();
                PlayerManager.instance.isInteract = true;
                ShowCursor();
            }

            if ((permission == MousePermission.All || permission == MousePermission.OnlyQuest)
                && Input.GetKeyDown(KeyCode.Q) && !PlayerManager.instance.isInteract)
            {
                QuestUI.Activate();
                QuestLogManager.instance.OpenQuestLog();
                PlayerManager.instance.DeactivateController();
                PlayerManager.instance.isInteract = true;
                ShowCursor();
                if (TutorialManager.Instance.isRunning)
                {
                    TutorialManager.Instance.currentUI.Deactivate();
                    TutorialManager.Instance.ShowNextStepDelayed();
                    permission = MousePermission.None; 

                }
            }
            if ((permission == MousePermission.All || permission == MousePermission.OnlyLeaderBoard)
                && Input.GetKeyDown(KeyCode.I) && !PlayerManager.instance.isInteract)
            {
                LeaderBoardUI.Activate();           
                PlayerManager.instance.DeactivateController();
                PlayerManager.instance.isInteract = true;
                ShowCursor();
            }
            if ((permission == MousePermission.All || permission == MousePermission.Inventory)
              && Input.GetKeyDown(KeyCode.M) && !PlayerManager.instance.isInteract)
            {
                InventoryUIManager.instance.OpenInventory();
                if (TutorialManager.Instance.isRunning)
                {
                    TutorialManager.Instance.currentUI.Deactivate();
                    TutorialManager.Instance.ShowNextStepDelayed();
                    permission = MousePermission.None;

                }
            }
           
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

    public void CloseQuestUI()
    {
        QuestUI.Deactivate();
        PlayerManager.instance.isInteract = false;
        PlayerManager.instance.ActivateController();
        HideCursor();
    }

    public void CloseLeaderBoardUI()
    {
        LeaderBoardUI.Deactivate();
        PlayerManager.instance.isInteract = false;
        PlayerManager.instance.ActivateController();
        HideCursor();
    }
}
