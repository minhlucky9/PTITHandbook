
using GameManager;
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
            QuestManager.instance.questMap[lootEvent.questId].OnQuestFinish += OnMainQuestComplete;
        };
        collectQuests.Add(lootEvent.minigameId, collectQuest);

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
        Debug.Log(currentCollected + "/" + numberToCollect);
        if(currentCollected == numberToCollect)
        {
            OnFinishQuest?.Invoke();
        }
    }
}
