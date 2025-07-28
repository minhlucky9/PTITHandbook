using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLootInteraction : LootInteraction
{
    string questId = "";
    public GameObject coinPrefab;
    public void SetupCoinMinigame(string questId)
    {
        this.questId = questId;
    }

    public override void OnEnterCollider()
    {

        base.OnEnterCollider();

        if(questId != "")
        {
            CollectQuestManager.instance.OnCollectQuestChange(questId);
        }
        AudioManager.instance.PlayCorrectSound();
     //   coinPrefab.SetActive(true);
        gameObject.SetActive(false);
    }
}
