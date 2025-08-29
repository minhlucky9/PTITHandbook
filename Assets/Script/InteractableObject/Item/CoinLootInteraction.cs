using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLootInteraction : LootInteraction
{
    string questId = "";
    public void SetupCoinMinigame(string questId)
    {
        this.questId = questId;
    }

    public override void OnEnterCollider()
    {

        base.OnEnterCollider();

        if(questId != "")
        {
            CollectQuestManager.instance.MarkCoinAsCollected(this);
            CollectQuestManager.instance.OnCollectQuestChange(questId);
        }
        AudioManager.instance.PlayCorrectSound();
        gameObject.SetActive(false);
    }
}
