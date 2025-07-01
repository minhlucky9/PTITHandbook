using GameManager;
using Interaction;
using Interaction.Minigame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropGameManager : MonoBehaviour
{
    public static DragAndDropGameManager instance;

    [HideInInspector] public Dictionary<string, DragAndDropQuest> quests = new Dictionary<string, DragAndDropQuest>();

    [Header("Slots & UI")]
    public List<InventorySlot> dropSlots;    // kéo tất cả các ô đích vào đây
    public Slider timerSlider;               // UI Slider 

    [Header("Thời gian")]
    public float totalTime = 60f;            
    private float timeRemaining;
    private int correctCount;
    private string questId;

    DialogConservation dialog = null;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
    
    }

    void Update()
    {
        if (timeRemaining <= 0) return;

        timeRemaining -= Time.deltaTime;
        timerSlider.value = timeRemaining;

        if (timeRemaining <= 0)
            EndGame(false);
    }


    public void InitDragAndDropGame(GameObject targetNPC, DragAndDropEventSO data)
    {
       StartCoroutine(ConservationManager.instance.ClearConservation());

        questId = data.questId;

        timeRemaining = totalTime;
        correctCount = 0;
        timerSlider.maxValue = totalTime;
        timerSlider.value = totalTime;

        foreach (var slot in dropSlots)
            slot.gameManager = this;



        StartCoroutine(DragAndDropUIManager.instance.ActivateMiniGameUI());

        var quest = new DragAndDropQuest
        {
           
            OnFinishQuest = () =>
            {
                targetNPC.SendMessage("OnQuestMinigameSuccess");

               
            }
        };
        quests.Add(questId, quest);

        Action listener = null;
        listener = () =>
        {
            if (correctCount >= dropSlots.Count)
            {
                quest.OnFinishQuest?.Invoke();
                EndGame(true);
            }
                
        };

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
        timeRemaining = 0f;

        // Vô hiệu hoá kéo thả toàn bộ
        foreach (var slot in dropSlots)
            foreach (Transform child in slot.transform)
                if (child.TryGetComponent<DraggableItem>(out var di))
                    di.enabled = false;

        if (win)
        {
            Debug.Log("Win");
            if (quests.TryGetValue(questId, out var quest))
                quest.OnFinishQuest?.Invoke();

            StartCoroutine(DragAndDropUIManager.instance.DeActivateMiniGameUI());

        }
        else
        {
            Debug.Log("Lose");
            var quest = QuestManager.instance.questMap[questId];
            quest.ChangeQuestState(QuestState.CAN_START);
            QuestManager.instance.InitQuestStep(
                quest.info.questSteps[quest.currentQuestStepIndex],
                questId
            );
        }
    }

    public class DragAndDropQuest
    {
  
        public Action OnFinishQuest;
    }

}
