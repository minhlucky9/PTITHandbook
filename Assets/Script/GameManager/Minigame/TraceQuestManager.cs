// TraceQuestManager.cs
using Interaction;
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
    private GameObject currentObject;
    private TraceQuest traceQuest;

    [Header("UI Progress")]
    [SerializeField] private UIAnimationController progressUI;
    [SerializeField] private TextMeshProUGUI progressText;

    void Awake()
    {
        instance = this;
    }

    public void InitTraceQuest(GameObject targetNPC, TraceEventSO traceEvent)
    {
        this.targetNPC = targetNPC;
        this.traceEvent = traceEvent;
        currentIndex = 0;
        

              // ===== Khởi tạo TraceQuest =====
        traceQuest = new TraceQuest();
        traceQuest.numberToTrace = traceEvent.traceObjects.Count;
               // Khi xong hết thì báo NPCController hoàn thành quest
        traceQuest.OnFinishQuest = () =>
        {
            targetNPC.SendMessage("OnQuestMinigameSuccess");
            ConservationManager.instance.StarContainer.Deactivate();
                   };
               // Hiển thị bar giống CollectQuestManager
        ConservationManager.instance.StarContainer.Activate();
        ConservationManager.instance.StarText.text = $"0/{traceQuest.numberToTrace}";

        SpawnNextObject();
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
            ConservationManager.instance.StarText.text =
                $"{currentTraced}/{numberToTrace}";
            Debug.Log($"{currentTraced}/{numberToTrace}");
            // Nếu đã tương tác đủ
            //if (currentTraced == numberToTrace)
            //    OnFinishQuest?.Invoke();
        }
    }
}
