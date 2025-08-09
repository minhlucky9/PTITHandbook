
using GameManager;
using Interaction;
using Interaction.Minigame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectQuestManager : MonoBehaviour
{
    public static CollectQuestManager instance;

    [HideInInspector] public Dictionary<string, CollectQuest> collectQuests = new Dictionary<string, CollectQuest>();
    public string currentCollectQuestId;
    GameObject container;
    GameObject targetNPC;
    CollectQuest __colectquest;
    LootEventSO lootEvent;
    private Coroutine CollectTimerRoutine;
    private float timeRemaining;
    private const float COLLECT_DURATION =300f;

    private void Awake()
    {
        instance = this;
    }

    public void InitCollectQuest(GameObject targetNPC, GameObject coinContainer, LootEventSO lootEvent)
    {
        container = coinContainer;
        timeRemaining = COLLECT_DURATION;
        this.targetNPC = targetNPC;
        this.lootEvent = lootEvent;
        currentCollectQuestId = lootEvent.questId;
        //
        CoinLootInteraction[] coins = coinContainer.GetComponentsInChildren<CoinLootInteraction>();
        foreach(CoinLootInteraction coin in coins)
        {
            coin.SetupCoinMinigame(lootEvent.minigameId);
        }

        //setup collect quest
        CollectQuest collectQuest = new CollectQuest();
        __colectquest = collectQuest;
        collectQuest.numberToCollect = lootEvent.numberOfLoot;
        collectQuest.OnFinishQuest = () => {
            targetNPC.SendMessage("FinishQuestStep");
            ConservationManager.instance.StarContainer.Deactivate();
            if(CollectTimerRoutine != null)
        {
                StopCoroutine(CollectTimerRoutine);
                CollectTimerRoutine = null;
            }

            // ẩn UI timer
            ConservationManager.instance.timerContainer.Deactivate();
            QuestManager.instance.questMap[lootEvent.questId].OnQuestFinish += OnMainQuestComplete;
        };
        collectQuests.Add(lootEvent.minigameId, collectQuest);
        ConservationManager.instance.StarContainer.Activate();
        ConservationManager.instance.StarText.text =
            $"0/{collectQuest.numberToCollect}";
        Invoke(nameof(StartCollectTimer), 0.5f);
        //setup quest complete callback

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
        Destroy(container);
        collectQuests.Remove(lootEvent.minigameId);
        // reset quest về CAN_START
        GameManager.QuestManager.instance.UpdateQuestStep(
         QuestState.CAN_START,
          currentCollectQuestId
      );

        targetNPC.SendMessage("ChangeNPCState", NPCState.HAVE_QUEST);

        targetNPC.SendMessage("OnQuizTimerFail");
    }

    public void OnMainQuestComplete()
    {
        Destroy(container);
    }

    public void OnCollectQuestChange(string minigameId)
    {
        CollectQuest quest = collectQuests[minigameId];
        quest.OnCollectedChange();
    }
}

public class CollectQuest
{
    public int numberToCollect;
    public int currentCollected;

    public Action OnFinishQuest;

    public void OnCollectedChange()
    {
        currentCollected++;
        ConservationManager.instance.StarText.text = $"{currentCollected}/{numberToCollect}";
        Debug.Log(currentCollected + "/" + numberToCollect);
        if(currentCollected == numberToCollect)
        {
            OnFinishQuest?.Invoke();
        }
    }
}
