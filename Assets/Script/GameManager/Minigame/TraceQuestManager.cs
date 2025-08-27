// TraceQuestManager.cs
using Interaction;
using Interaction.Minigame;
using PlayerController;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TraceQuestManager : MonoBehaviour
{
    public static TraceQuestManager instance;

    private GameObject targetNPC;
    private TraceEventSO traceEvent;
    private int currentIndex = 0;
    public GameObject currentObject;
    private TraceQuest traceQuest;

    [Header("UI Progress")]
    [SerializeField] private UIAnimationController progressUI;
    [SerializeField] private TextMeshProUGUI progressText;

    private Coroutine CollectTimerRoutine;
    private float timeRemaining;
    private const float COLLECT_DURATION = 20f;

    void Awake()
    {
        instance = this;
    }

    public void InitTraceQuest(GameObject targetNPC, TraceEventSO traceEvent)
    {
        this.targetNPC = targetNPC;
        this.traceEvent = traceEvent;
        currentIndex = 0;
        timeRemaining = COLLECT_DURATION;

        // ===== Khởi tạo TraceQuest =====
        traceQuest = new TraceQuest();
        traceQuest.numberToTrace = traceEvent.traceObjects.Count;
               // Khi xong hết thì báo NPCController hoàn thành quest
        traceQuest.OnFinishQuest = () =>
        {
            targetNPC.SendMessage("OnQuestMinigameSuccess");
            ConservationManager.instance.StarContainer.Deactivate();
            if (CollectTimerRoutine != null)
            {
                StopCoroutine(CollectTimerRoutine);
                CollectTimerRoutine = null;
            }
            ConservationManager.instance.timerContainer.Deactivate();
        };
               // Hiển thị bar giống CollectQuestManager
        ConservationManager.instance.StarContainer.Activate();
        ConservationManager.instance.StarText.text = "Số vật phẩm đã thu thập: " + $"0/{traceQuest.numberToTrace}";
        SpawnNextObject();
        Invoke(nameof(StartCollectTimer), 0.5f);
    }

    private void StartCollectTimer()
    {
        // Tránh gọi nhiều lần
        if (CollectTimerRoutine != null)
        {
            StopCoroutine(CollectTimerRoutine);
        }

        CollectTimerRoutine = StartCoroutine(CollectCountdown(timeRemaining));
    }

    private IEnumerator CollectCountdown(float duration)
    {
        float t = duration;
        ConservationManager.instance.timerContainer.Activate();


        while (t > 0f)
        {
            // tính phút và giây
            int minutes = (int)(t / 60);
            int seconds = (int)(t % 60);
            // format “MM:SS”
            ConservationManager.instance.timerText.text = $"{minutes:00}:{seconds:00}";

            t -= Time.deltaTime;
            timeRemaining = t;
            yield return null;
        }

        // khi hết giờ
        ConservationManager.instance.timerText.text = "00:00";
        OnCollectTimerExpired();
    }

    private void OnCollectTimerExpired()
    {
        // dừng coroutine nếu còn chạy
        if (CollectTimerRoutine != null)
        {
            StopCoroutine(CollectTimerRoutine);
            CollectTimerRoutine = null;
        }

        // ẩn UI timer
        ConservationManager.instance.timerContainer.Deactivate();
        ConservationManager.instance.StarContainer.Deactivate();
        // reset quest về CAN_START
        GameManager.QuestManager.instance.UpdateQuestStep(
         QuestState.CAN_START,
          traceEvent.questId
      );

        ForceCloseAllActiveUI();


        targetNPC.SendMessage("ChangeNPCState", NPCState.HAVE_QUEST);

        DialogConservation correctDialog = new DialogConservation();
        DialogResponse response = new DialogResponse();

        correctDialog.message = "Thời gian đã hết. Bạn đã không thể hoàn thành nhiệm vụ. Hãy thử lại vào lần tới";
        response.executedFunction = DialogExecuteFunction.OnQuestMinigameFail;

        response.message = "Đã hiểu";
        correctDialog.possibleResponses.Add(response);
        TalkInteraction.instance.StartCoroutine(TalkInteraction.instance.SmoothTransitionToTraceMiniGame());
        StartCoroutine(ConservationManager.instance.UpdateConservation(correctDialog));

        targetNPC.SendMessage("OnQuizTimerFail");
    }

    /// <summary>
    /// Tắt tất cả UIAnimationController đang active trong scene
    /// </summary>
    private void ForceCloseAllActiveUI()
    {
        // Tìm tất cả TraceObjectInteraction trong scene
        TraceObjectInteraction[] allTraceObjects = FindObjectsOfType<TraceObjectInteraction>();

        foreach (TraceObjectInteraction traceObj in allTraceObjects)
        {
            if (traceObj != null && traceObj.uiControllers != null)
            {
                // Tắt tất cả UI controllers của object này
                foreach (UIAnimationController uiController in traceObj.uiControllers)
                {
                    if (uiController != null)
                    {
                        uiController.Deactivate();
                    }
                }
            }
          
        }

        // Kích hoạt lại player controller nếu bị disable
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.ActivateController();
        }
    }

    private void SpawnNextObject()
    {
        if (currentIndex >= traceEvent.traceObjects.Count)
            return;

        var prefab = traceEvent.traceObjects[currentIndex].prefab;
        currentObject = Instantiate(prefab);
        var ti = currentObject.AddComponent<TraceObjectInteraction>();
        ti.Initialize(currentIndex);
    }

    /// <summary>
    /// Khi interaction được “xác nhận” sau UIAnimationController đóng
    /// </summary>
    public void ConfirmTrace(int index)
    {
        if (currentObject) Destroy(currentObject);

        // Tăng counter và cập nhật UI
        traceQuest.OnTracedChange();
        if(currentIndex+1 >= traceEvent.traceObjects.Count)
        {
            targetNPC.SendMessage("OnQuestMinigameSuccess");
            ConservationManager.instance.StarContainer.Deactivate();
        }
        else
        {
            targetNPC.SendMessage("FinishQuestStep");
        }
      
        currentIndex++;
        SpawnNextObject();
       
    }

    public void OnTraceObjectInteracted(string questId, int index)
    {
        // destroy object vừa tương tác
        if (currentObject) Destroy(currentObject);

        // thông báo NPCController hoàn thành step
        targetNPC.SendMessage("OnQuestMinigameSuccess");

        // tăng index và spawn vật tiếp theo
        currentIndex++;
        SpawnNextObject();
    }

    public class TraceQuest
    {
        public int numberToTrace;
        public int currentTraced;
        public Action OnFinishQuest;

        /// <summary>
        /// Gọi mỗi khi một vật phẩm được “xác nhận” tương tác
        /// sẽ tăng counter và cập nhật UI giống CollectQuest.OnCollectedChange()
        /// </summary>
        public void OnTracedChange()
        {
            currentTraced++;
            // Cập nhật text dạng "1/6", "2/6", …
            ConservationManager.instance.StarText.text = "Số vật phẩm đã thu thập: " + $"{currentTraced}/{numberToTrace}";
            Debug.Log($"{currentTraced}/{numberToTrace}");
            // Nếu đã tương tác đủ
            //if (currentTraced == numberToTrace)
            //    OnFinishQuest?.Invoke();
        }
    }
}
