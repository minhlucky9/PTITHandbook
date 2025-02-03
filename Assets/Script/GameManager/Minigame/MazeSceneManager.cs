using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeSceneManager : SubSceneGameManager
{
    public CoinLootInteraction[] targets;

    public override void InitGame(GameObject targetNPC, MinigameDataSO minigameData, string sceneName)
    {
        base.InitGame(targetNPC, minigameData, sceneName);
        //
        foreach (CoinLootInteraction coin in targets)
        {
            coin.SetupCoinMinigame(minigameData.minigameId);
        }

        //setup collect quest
        CollectQuest collectQuest = new CollectQuest();
        collectQuest.numberToCollect = targets.Length;
        collectQuest.OnFinishQuest = () => {
            targetNPC.SendMessage("OnQuestMinigameSuccess");
            //
            StartCoroutine(ExitScene(3f));
        };
        CollectQuestManager.instance.collectQuests.Add(minigameData.minigameId, collectQuest);
    }

    IEnumerator ExitScene(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
