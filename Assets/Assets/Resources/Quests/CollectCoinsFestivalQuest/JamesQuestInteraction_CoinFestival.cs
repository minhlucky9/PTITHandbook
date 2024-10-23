using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalResponseData_Login;
using static CollectCoinsQuestStep;

public class JamesInteraction_CoinFestival : MonoBehaviour
{
   // public CollectCoinsQuestStep step;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForJames;
    public StarFestival star;


    [Header("UI")]
   // [SerializeField] private GameObject InteractUI;
    [SerializeField] private GameObject questStartUI;
    [SerializeField] private GameObject questProgressUI;
    [SerializeField] private GameObject questCompleteUI;
    [SerializeField] private GameObject ClaimPrizeUI;
    public GameObject uiToShowAfterFail;
    public GameObject uiFailedTimerUI;
    [SerializeField] private Button startQuestButton;
    [SerializeField] private Button completeQuestButton;
    private bool playerIsNear = false;
    public QuestPoint QuizCoinPoint;

    [Header("Progress")]
    [SerializeField] private GameObject InProgressUI;
    [SerializeField] private GameObject CarsUI;
    [SerializeField] private GameObject StatusQuestUI;
    public GameObject uiToShow; // UI cần hiện
    public GameObject uiToShowAfter; // UI hiện sau 5 phút

    private bool questStarted = false;
    private bool questCompleted = false;

    public TextMeshProUGUI timerText; // Text để hiển thị thời gian
    public TextMeshProUGUI failTimerText;

    private float timeRemaining = 300f; // 5 phút = 300 giây
    private float failTimer = 300f;
    private bool isTimerRunning = false;
    private bool isFalsedTimerRunning = false;

    

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
                // Khi hết thời gian
                timeRemaining = 0;
                isTimerRunning = false;
                SwitchUI();
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
                // Khi hết thời gian
                failTimer = 0;
                isFalsedTimerRunning = false;
                // SwitchUI();
            }
        }

        if ((QuizCoinPoint.currentQuestState.Equals(QuestState.FINISHED)) && GlobalResponseData.quests != null)
        {
            questCompleted = true;
            //   questCompleteUI.SetActive(true);
            foreach (var key in GlobalResponseData.quests.Keys)
            {

                if (key == "CollectCoinsFestivalQuest")
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
        else if (QuizCoinPoint.currentQuestState.Equals(QuestState.IN_PROGRESS))
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
        if (QuizCoinPoint.currentQuestState.Equals(QuestState.IN_PROGRESS))
        {
            InProgressUI.SetActive(true);
            questProgressUI.SetActive(true);
            CarsUI.SetActive(false);
            timeRemaining = 300f;
            GameEventsManager.instance.questEvents.StartQuest(questInfoForJames.id);
        }
       else if (QuizCoinPoint.currentQuestState.Equals(QuestState.FAILED))
        {
            uiFailedTimerUI.SetActive(true);
            failTimer = 300f;
            GameEventsManager.instance.questEvents.CancelQuest(questInfoForJames.id);
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
                InProgressUI.SetActive(false);
                questCompleteUI.SetActive(true);
                questCompleted = true;
            }
            else if (quest.state == QuestState.FINISHED)
            {
                CarsUI.SetActive(true);
                questProgressUI.SetActive(false);
                questCompleted = false ;
                ClaimPrizeUI.SetActive(true);
            }
            else if (quest.state == QuestState.FAILED)
            {
                star.star = 0;
                questCompleted = false;
                CarsUI.SetActive(true);
                isFalsedTimerRunning = true;
                timeRemaining = 300f;
            }
            else if (quest.state == QuestState.CAN_START)
            {
                uiFailedTimerUI.SetActive(false);
                questStartUI.SetActive(true);
                uiToShowAfterFail.SetActive(true);
                failTimer = 300f;

            }
            else if (quest.state == QuestState.IN_PROGRESS)
            {
                CarsUI.SetActive(false);
                uiToShowAfterFail.SetActive(false);
               isTimerRunning = true;
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
        CarsUI.SetActive(false);
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
        // Tính toán phút và giây còn lại
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = "Thời gian còn lại: " + $"{minutes:00}:{seconds:00}";
    }
    private void UpdateFailTimerDisplay()
    {
        // Tính toán phút và giây còn lại
        int minutes = Mathf.FloorToInt(failTimer / 60);
        int seconds = Mathf.FloorToInt(failTimer % 60);
        failTimerText.text = "Thời gian còn lại: " + $"{minutes:00}:{seconds:00}";
    }

    private void SwitchUI()
    {
        QuizCoinPoint.FailQuest();
        // Ẩn UI hiện tại và hiện UI mới
        uiToShow.SetActive(false);
        uiToShowAfter.SetActive(true);
       
    }
}
