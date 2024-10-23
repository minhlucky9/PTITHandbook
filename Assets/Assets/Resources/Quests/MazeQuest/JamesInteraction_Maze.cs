using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalResponseData_Login;

public class JamesInteraction_Mazw : MonoBehaviour
{
    // public CollectCoinsQuestStep step;

    [Header("UIContainer")]
    [SerializeField] private GameObject MazeUI;
    [SerializeField] private GameObject[] MazeStarUI;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForJames;
    public Maze star;

    [Header("UI")]
    // [SerializeField] private GameObject InteractUI;
    [SerializeField] private GameObject questStartUI;
    [SerializeField] private GameObject questProgressUI;
    [SerializeField] private GameObject UIIngame;
    [SerializeField] private GameObject questCompleteUI;
    [SerializeField] private GameObject FailedUI;
    [SerializeField] private GameObject FailedTimeUI;
    [SerializeField] private GameObject TimeUI;
    [SerializeField] private GameObject ClaimPrizeUI;
    [SerializeField] private Button startQuestButton;
    [SerializeField] private Button completeQuestButton;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI failTimerText;
    private bool playerIsNear = false;
    public QuestPoint QuizCoinPoint;
    public FastTravel fastTravel;

    [Header("Progress")]
    [SerializeField] private GameObject InProgressUI;
    [SerializeField] private GameObject StatusQuestUI;

    [Header("Notification")]
    [SerializeField] private GameObject NotifForEscapeMazeUI;
    [SerializeField] private GameObject NPCNotifForEscapeMazeUI;
    [SerializeField] private GameObject NPCNotifForStayinMazeUI;

    [Header("Time")]
    private float timeRemaining = 25f; // 5 phút = 300 giây
    public float failTimer = 15f;
    private bool isTimerRunning = false;
    public bool isFalsedTimerRunning = false;

    [Header("MovementFrezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;
    public MouseManager MouseManager;
    public ButtonActivator Activator;


    private bool questStarted = false;
    private bool questCompleted = false;
    private bool IsFinish = false;


    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                // Khi h?t th?i gian
                timeRemaining = 0;
                isTimerRunning = false;
                if (!IsFinish)
                {
                    FailedUI.SetActive(true);
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
                
                //SwitchUI();
            }

        }
        if (isFalsedTimerRunning)
        {
            if (failTimer > 0)
            {
                failTimer -= Time.deltaTime;
                UpdateFailTimerDisplay();
            }
            else
            {
                // Khi h?t th?i gian
                failTimer = 0;
                isFalsedTimerRunning = false;
                Debug.Log("isFalsedTimerRunning is now set to false.");

            }
        }
        if ((QuizCoinPoint.currentQuestState.Equals(QuestState.FINISHED)) && GlobalResponseData.quests != null)
        {
            //questCompleted = true;
            //   questCompleteUI.SetActive(true);
            foreach (var key in GlobalResponseData.quests.Keys)
            {

                if (key == "MazeQuest")
                {
                    Quest quest = GlobalResponseData.quests[key];

                    // Gi? s? b?n mu?n l?y giá tr? c?a questStepIndex ho?c c?a m?t QuestStepState c? th?
                    if (quest.currentQuestStepIndex >= 0 && quest.currentQuestStepIndex < quest.questStepStates.Length)
                    {
                        QuestStepState stepState = quest.questStepStates[quest.currentQuestStepIndex];

                        // Chuy?n ??i `state` thành `int` n?u có th?
                        if (int.TryParse(stepState.state, out int stateValue))
                        {
                            //  step.coinsCollected = stateValue;
                            star.star = stateValue;
                            Debug.Log($"QuizA1.score updated to: {stateValue} based on QuestStepState.state");
                        }
                        else
                        {
                            Debug.LogWarning($"Cannot convert QuestStepState.state '{stepState.state}' to an integer.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid quest step index: {quest.currentQuestStepIndex}");
                    }

                    break; // Thoát kh?i vòng l?p sau khi tìm th?y key phù h?p
                }
            }
            // QuizA1.instance.GameOver();
        }
        else if (QuizCoinPoint.currentQuestState.Equals(QuestState.CAN_START))
        {
            questStartUI.SetActive(true);
        }
        else if (QuizCoinPoint.currentQuestState.Equals(QuestState.CAN_FINISH))
        {

        }

        // Debug.Log(QuizCoinPoint.currentQuestState);
    }

    private void Awake()
    {
        // questStartUI.SetActive(false);
        //  questCompleteUI.SetActive(false);

        startQuestButton.onClick.AddListener(StartQuest);
        completeQuestButton.onClick.AddListener(CompleteQuest);
    }


    private void Start()
    {
        StartCoroutine(CallAfterFirstFrame());


    }

    IEnumerator CallAfterFirstFrame()
    {
        // Đợi đến frame tiếp theo
        yield return null;

        // Gọi hàm bạn muốn sau khi đã qua frame đầu tiên
        YourMethod();
    }

    void YourMethod()
    {
        if (QuizCoinPoint.currentQuestState.Equals(QuestState.FAILED))
        {
            for (int i = 0; i < 2; i++)
            {
                MazeStarUI[i].SetActive(true);
            }
            FailedTimeUI.SetActive(true);
            
            failTimer = 15f;
            GameEventsManager.instance.questEvents.CancelQuest(questInfoForJames.id);
        }
       else if (QuizCoinPoint.currentQuestState.Equals(QuestState.CAN_FINISH))
        {
            questProgressUI.SetActive(true);
            
           
        }
        else if (QuizCoinPoint.currentQuestState.Equals(QuestState.FINISHED))
        {
            ClaimPrizeUI.SetActive(true);

        }
        else if (QuizCoinPoint.currentQuestState.Equals(QuestState.IN_PROGRESS))
        {
            fastTravel.TravelTo(1);

        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed += OnSubmitPressed;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed -= OnSubmitPressed;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id == questInfoForJames.id)
        {
            if (quest.state == QuestState.CAN_FINISH)
            {
                MouseManager.ShowCursor();
                Activator.IsUIShow = true;
                InProgressUI.SetActive(false);
                questCompleteUI.SetActive(true);
                MazeUI.SetActive(true);
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
                NotifForEscapeMazeUI.SetActive(true);
                NPCNotifForEscapeMazeUI.SetActive(true);
                NPCNotifForStayinMazeUI.SetActive(false);
                UIIngame.SetActive(false);
                questCompleted = true;
                timeRemaining = 25f;
                isTimerRunning = false;
            }
            else if (quest.state == QuestState.FINISHED)
            {
                questCompleted = false;
                IsFinish = true;
               
              //  ClaimPrizeUI.SetActive(true);
                
            }
            else if (quest.state == QuestState.IN_PROGRESS)
            {

               isTimerRunning = true;
                InProgressUI.SetActive(true);
                questProgressUI.SetActive(true);
                NPCNotifForEscapeMazeUI.SetActive(false);
                NPCNotifForStayinMazeUI.SetActive(true);
                TimeUI.SetActive(true);
            }
            else if (quest.state == QuestState.FAILED)
            {
                star.star = 0;
               isFalsedTimerRunning = true;
                timeRemaining = 25f;
               questCompleteUI.SetActive(false);
                questCompleted = false;
                FailedTimeUI.SetActive(true);
                InProgressUI.SetActive(false);
                for(int i = 0; i < 2; i++)
                {
                    MazeStarUI[i].SetActive(true);
                }
                GameEventsManager.instance.miscEvents.MazeRetry();
            }
            else if (quest.state == QuestState.CAN_START)
            {
                questStartUI.SetActive(true);
                FailedTimeUI.SetActive(false);
                failTimer = 15f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    private void OnSubmitPressed()
    {
        if (playerIsNear)
        {
            if (questCompleted)
            {
                questCompleteUI.SetActive(true);
            }
            else
            {
                questStartUI.SetActive(true);
            }
        }
    }

    private void OnQuestFinished(string questId)
    {
        // Assuming this is the specific quest ID you want to track
        if (questId == "CollectCoinsQuest")
        {
            questCompleted = true;
        }
    }

    private void StartQuest()
    {
        //   questStarted = true;
        //   InteractUI.SetActive(false);
        questStartUI.SetActive(false);
        questProgressUI.SetActive(true);
        InProgressUI.SetActive(true);
       // StatusQuestUI.SetActive(true);
        //questCompleteUI.SetActive(true);
        GameEventsManager.instance.questEvents.StartQuest(questInfoForJames.id);
    }

    private void CompleteQuest()
    {
        if (questCompleted)
        {
            //questStartUI.SetActive(false);
            // questCompleteUI.SetActive(true);
            GameEventsManager.instance.questEvents.FinishQuest(questInfoForJames.id);
            //  questProgressUI.SetActive(false);

        }

        // questCompleteUI.SetActive(false);



    }

    private void UpdateTimerDisplay()
    {
        // Tính toán phút và giây còn l?i
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    private void UpdateFailTimerDisplay()
    {
        // Tính toán phút và giây còn l?i
        int minutes = Mathf.FloorToInt(failTimer / 60);
        int seconds = Mathf.FloorToInt(failTimer % 60);
        failTimerText.text = $"{minutes:00}:{seconds:00}";
    }


    public void DisableTime()
    {
        TimeUI.SetActive(false);
    }

    public void CheckPoint()
    {
        if (QuizCoinPoint.currentQuestState.Equals(QuestState.IN_PROGRESS))
        {
            fastTravel.TravelTo(2);
            QuizCoinPoint.FailQuest();
            isTimerRunning = false;
           // timeRemaining = 25f;
        }
    }
}
