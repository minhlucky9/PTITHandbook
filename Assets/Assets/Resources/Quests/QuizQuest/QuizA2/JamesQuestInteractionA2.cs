
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static GlobalResponseData_Login;
using UnityEngine.UI;
using TMPro;

public class JamesInteraction_QuizA2 : MonoBehaviour
{
    public QuizA2 QuizA2;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForJames;

    [Header("UI")]
    public GameObject questStartUI;
    //[SerializeField] private GameObject questProgressUI;
    [SerializeField] private GameObject questCompleteUI;
    [SerializeField] private GameObject ClaimPrizeUI;
    [SerializeField] private Button startQuestButton;
    [SerializeField] private Button completeQuestButton;

    public GameObject uiToShow; // UI cần hiện
    public GameObject uiToShowAfter; // UI hiện sau 5 phút
    public GameObject uiToShowAfterFail;
    public GameObject uiFailedTimerUI;
    public TextMeshProUGUI timerText; // Text để hiển thị thời gian
    public TextMeshProUGUI failTimerText;

    private float timeRemaining = 300f; // 5 phút = 300 giây
    private float failTimer = 300f;
    private bool isTimerRunning = false;
    private bool isFalsedTimerRunning = false;

    public QuestPoint QuizA2Point;
    private bool playerIsNear = false;

    [Header("QuestLog")]
    public Slider QuizA2Slider;
    public TextMeshProUGUI QuizA2QuestLog;

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

        if ((QuizA2Point.currentQuestState.Equals(QuestState.FINISHED)) && GlobalResponseData.quests != null)
        {
            //questCompleted = true;
            //   questCompleteUI.SetActive(true);
            foreach (var key in GlobalResponseData.quests.Keys)
            {

                if (key == "QuestQuizA2SO")
                {
                    Quest quest = GlobalResponseData.quests[key];

                    // Giả sử bạn muốn lấy giá trị của questStepIndex hoặc của một QuestStepState cụ thể
                    if (quest.currentQuestStepIndex >= 0 && quest.currentQuestStepIndex < quest.questStepStates.Length)
                    {
                        QuestStepState stepState = quest.questStepStates[quest.currentQuestStepIndex];

                        // Chuyển đổi `state` thành `int` nếu có thể
                        if (int.TryParse(stepState.state, out int stateValue))
                        {
                            QuizA2.score = stateValue;
                            Debug.Log($"QuizA1.score updated to: {QuizA2.score} based on QuestStepState.state");
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

                    break; // Thoát khỏi vòng lặp sau khi tìm thấy key phù hợp
                }
            }
            //QuizA1.instance.GameOver();
        }
        else if (QuizA2Point.currentQuestState.Equals(QuestState.CAN_START))
        {
            questStartUI.SetActive(true);
        }
        else if (QuizA2Point.currentQuestState.Equals(QuestState.FAILED))
        {
            
        }
        Debug.Log(QuizA2Point.currentQuestState);
    }

 //   private bool questStarted = false;
    [SerializeField]
    private bool questCompleted = false;

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
        if (QuizA2Point.currentQuestState.Equals(QuestState.FAILED))
        {
            uiFailedTimerUI.SetActive(true);
            failTimer = 300f;
            GameEventsManager.instance.questEvents.CancelQuest(questInfoForJames.id);
        }
        if (QuizA2Point.currentQuestState.Equals(QuestState.FINISHED))
        {
            QuizA2Slider.value = QuizA2.GetScore();
            QuizA2QuestLog.text = QuizA2.GetScore().ToString() + "/8";
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
                questCompleted = true;
            }
            else if (quest.state == QuestState.FINISHED)
            {
                QuizA2Slider.value = QuizA2.GetScore();
                QuizA2QuestLog.text = QuizA2.GetScore().ToString() + "/8";
                questCompleted = false;
                ClaimPrizeUI.SetActive(true);
                questCompleteUI.SetActive(false);
            }
            else if (quest.state == QuestState.FAILED)
            {
                QuizA2Slider.value = QuizA2.GetScore();
                QuizA2QuestLog.text = QuizA2.GetScore().ToString() + "/8";
                uiFailedTimerUI.SetActive(true);
                questCompleted = false;
                QuizA2.RestartGame();   
                QuizA2.isTimerRunning = false;
                isFalsedTimerRunning = true;
                timeRemaining = 300f;
            }
            else if (quest.state == QuestState.CAN_START)
            {
                QuizA2Slider.value = QuizA2.GetScore();
                QuizA2QuestLog.text = QuizA2.GetScore().ToString() + "/8";
                uiFailedTimerUI.SetActive(false);
                questStartUI.SetActive(true);
                uiToShowAfterFail.SetActive(true);
                questCompleteUI.SetActive(false);
                isFalsedTimerRunning = false;
                failTimer = 300f;
              
            }
            else if (quest.state == QuestState.IN_PROGRESS)
            {
                uiToShowAfterFail.SetActive(false);

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
               // questCompleteUI.SetActive(true);
                //QuizA1.instance.GameOver();
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

    public void StartQuest()
    {
     //   questStarted = true;
     //   questStartUI.SetActive(false);
    //    questProgressUI.SetActive(true);
        GameEventsManager.instance.questEvents.StartQuest(questInfoForJames.id);
        QuizA2.isTimerRunning = true;
        isTimerRunning = false;
    }

    private void CompleteQuest()
    {
        if (questCompleted)
        {
          //  questStartUI.SetActive(false);
          //  questCompleteUI.SetActive(true);
            GameEventsManager.instance.questEvents.FinishQuest(questInfoForJames.id);
          //  questProgressUI.SetActive(false);
        }
      
       // questCompleteUI.SetActive(false);



    }

    public void ActivateUI()
    {
        uiToShow.SetActive(true);
        uiToShowAfter.SetActive(false);
        isTimerRunning = true;
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
        // Ẩn UI hiện tại và hiện UI mới
        uiToShow.SetActive(false);
        uiToShowAfter.SetActive(true);
    }
}
