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
    private TraceObjectInteraction DragAndDropPuzzle;

    [Header("UI Progress")]
    [SerializeField] private UIAnimationController progressUI;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Slots & UI")]
    public List<InventorySlot> dropSlots;
    private int correctCount;

    private Coroutine CollectTimerRoutine;
    private float timeRemaining;
    private const float COLLECT_DURATION = 300f;


    private List<ItemInitialState> initialStates = new List<ItemInitialState>();

    [System.Serializable]
    public class ItemInitialState
    {
        public DraggableItem item;
        public Transform originalParent;
        public Vector3 originalPosition;
        public bool wasEnabled;
    }

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
        EndGame(false);

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
            targetNPC.SendMessage("FinishQuestStep");
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

    #region Drag and Drop Puzzle

    private void ResetGameState()
    {
        // Lưu trạng thái ban đầu nếu chưa có
        if (initialStates.Count == 0)
        {
            SaveInitialStates();
        }

        // Reset tất cả items về vị trí ban đầu
        foreach (var state in initialStates)
        {
            if (state.item != null)
            {
                state.item.transform.SetParent(state.originalParent);
                state.item.transform.localPosition = state.originalPosition;
                state.item.transform.localRotation = Quaternion.identity;
                state.item.enabled = state.wasEnabled;
                state.item.image.raycastTarget = true;
            }
        }

        correctCount = 0;
    }

    // Lưu trạng thái ban đầu của tất cả draggable items
    private void SaveInitialStates()
    {
        initialStates.Clear();

        // Tìm tất cả DraggableItem trong scene
        DraggableItem[] allItems = FindObjectsOfType<DraggableItem>();

        foreach (var item in allItems)
        {
            ItemInitialState state = new ItemInitialState
            {
                item = item,
                originalParent = item.transform.parent,
                originalPosition = item.transform.localPosition,
                wasEnabled = item.enabled
            };
            initialStates.Add(state);
        }
    }

    // Gọi khi thả đúng
    public void OnCorrectDrop(DraggableItem item)
    {
        correctCount++;
        item.enabled = false;

        if (correctCount >= dropSlots.Count)
            EndGame(true);
    }

    // Gọi khi thả sai (tuỳ feedback)
    public void OnWrongDrop(DraggableItem item)
    {
        Debug.Log($"Wrong {item.itemID}");
    }

    // Kết thúc game: win=true hoặc false
    private void EndGame(bool win)
    {
 

        // Vô hiệu hoá kéo thả toàn bộ
        foreach (var slot in dropSlots)
            foreach (Transform child in slot.transform)
                if (child.TryGetComponent<DraggableItem>(out var di))
                    di.enabled = false;

        if (win)
        {
            Debug.Log("Win");
            
           

            StartCoroutine(DragAndDropUIManager.instance.DeActivateMiniGameUI());

            StartCoroutine(DelayAfterFinishPuzzle());

        }
        else
        {
            Debug.Log("Lose");
           
           
          
            StartCoroutine(DragAndDropUIManager.instance.DeActivateMiniGameUI());
           
        }
    }

    public void InitPuzzle()
    {
        ResetGameState();

        foreach (var slot in dropSlots)
            slot.gameManager = this;



        StartCoroutine(DragAndDropUIManager.instance.ActivateMiniGameUI());
    }

    private IEnumerator DelayAfterFinishPuzzle()
    {
        yield return new WaitForSeconds(1f);

        DragAndDropPuzzle = FindAnyObjectByType<TraceObjectInteraction>();

        if (DragAndDropPuzzle != null)
        {
            DragAndDropPuzzle.OnClick_NextUI();
        }
    }

    #endregion

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
