
using GameManager;
using Interaction;
using Interaction.Minigame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectQuestManager : MonoBehaviour
{
    public static CollectQuestManager instance;

    [HideInInspector]public Dictionary<string, CollectQuest> collectQuests = new Dictionary<string, CollectQuest>();
    GameObject container;

    private void Awake()
    {
        instance = this;
    }

    public void InitCollectQuest(GameObject targetNPC, GameObject coinContainer, LootEventSO lootEvent)
    {
        container = coinContainer;
        //
        CoinLootInteraction[] coins = coinContainer.GetComponentsInChildren<CoinLootInteraction>();
        foreach(CoinLootInteraction coin in coins)
        {
            coin.SetupCoinMinigame(lootEvent.minigameId);
        }

        //setup collect quest
        CollectQuest collectQuest = new CollectQuest();
        collectQuest.numberToCollect = lootEvent.numberOfLoot;
        collectQuest.OnFinishQuest = () => {
            targetNPC.SendMessage("OnQuestMinigameSuccess");
            ConservationManager.instance.StarContainer.Deactivate();
            QuestManager.instance.questMap[lootEvent.questId].OnQuestFinish += OnMainQuestComplete;
        };
        collectQuests.Add(lootEvent.minigameId, collectQuest);
        ConservationManager.instance.StarContainer.Activate();
        ConservationManager.instance.StarText.text =
            $"0/{collectQuest.numberToCollect}";
        //setup quest complete callback

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
