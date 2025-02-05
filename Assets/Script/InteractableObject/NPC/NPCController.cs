using GameManager;
using Interaction;
using Interaction.Minigame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : TalkInteraction
{
    [Header("NPC infos")]
    public NPCInfoSO npcInfo;
    public NPCState currentNPCState;
    Dictionary<string, NPCConservationSO> npcConservationMap;
    Dictionary<string, MinigameDataSO> minigameDataMap;
    Dictionary<string, FunctionalWindowDataSO> functionalDataMap;

    [HideInInspector] 
    public NPCConservationSO currentDialogConservation;
    QuestManager questManager;
    [Header("Quest Mission")]
    string questId;
    QuestStep questStep;

    [Header("State Symbol")]
    public GameObject iconDefault;
    public GameObject iconQuestAvailable;
    public GameObject iconQuestComplete;
    public GameObject iconQuestInProgress;

    public override void Awake()
    {
        base.Awake();
        questManager = FindAnyObjectByType<QuestManager>();
        //create data map
        npcConservationMap = CreateNPCConservationMap();
        minigameDataMap = CreateMinigameDataMap();
        functionalDataMap = CreateFunctionalDataMap();
        //
        currentDialogConservation = npcInfo.normalConservation;
    }

    public override void Interact()
    {
        conservationManager.InitConservation(gameObject, currentDialogConservation);
        base.Interact();
    }

    public void OpenShopFunctionalWindow()
    {
        string functionId = FunctionType.Shop.ToString() + "_" + npcInfo.npcId;
        //find minigame in minigame map
        FunctionalWindowDataSO functionWindowData = functionalDataMap[functionId];
        functionWindowData.Init(gameObject);
    }

    #region NPC Data
    private Dictionary<string, NPCConservationSO> CreateNPCConservationMap()
    {
        // loads all QuestInfo Scriptable Objects under the Assets/Resources/Quests folder
        NPCConservationSO[] allConservations = Resources.LoadAll<NPCConservationSO>("NPC/" + npcInfo.npcId );

        // Create the quest map
        Dictionary<string, NPCConservationSO> npcConservationMap = new Dictionary<string, NPCConservationSO>();
        foreach (NPCConservationSO npcConservation in allConservations)
        {
            if (npcConservationMap.ContainsKey(npcConservation.conservationId))
            {
                Debug.LogWarning("Duplicate ID found when creating npc conservation map: " + npcConservation.conservationId);
            }
            npcConservationMap.Add(npcConservation.conservationId, npcConservation);
        }
        return npcConservationMap;
    }

    private Dictionary<string, MinigameDataSO> CreateMinigameDataMap()
    {
        // loads all QuestInfo Scriptable Objects under the Assets/Resources/Quests folder
        MinigameDataSO[] allMinigames = Resources.LoadAll<MinigameDataSO>("NPC/" + npcInfo.npcId);

        // Create the quest map
        Dictionary<string, MinigameDataSO> npcMinigameDataMap = new Dictionary<string, MinigameDataSO>();
        foreach (MinigameDataSO minigame in allMinigames)
        {
            if (npcMinigameDataMap.ContainsKey(minigame.minigameId))
            {
                Debug.LogWarning("Duplicate ID found when creating minigame data map: " + minigame.minigameId);
            }
            npcMinigameDataMap.Add(minigame.minigameId, minigame);
        }
        return npcMinigameDataMap;
    }

    private Dictionary<string, FunctionalWindowDataSO> CreateFunctionalDataMap()
    {
        // loads all QuestInfo Scriptable Objects under the Assets/Resources/Quests folder
        FunctionalWindowDataSO[] allFunctionals = Resources.LoadAll<FunctionalWindowDataSO>("NPC/" + npcInfo.npcId);

        // Create the quest map
        Dictionary<string, FunctionalWindowDataSO> npcFunctionalDataMap = new Dictionary<string, FunctionalWindowDataSO>();
        foreach (FunctionalWindowDataSO functional in allFunctionals)
        {
            if (npcFunctionalDataMap.ContainsKey(functional.functionId))
            {
                Debug.LogWarning("Duplicate ID found when creating minigame data map: " + functional.functionId);
            }
            npcFunctionalDataMap.Add(functional.functionId, functional);
        }
        return npcFunctionalDataMap;
    }
    #endregion

    #region Quest Minigame Handle
    public void StartQuestMinigame()
    {
        switch (questStep.missionType)
        {
            case StepMissionType.TALKING:
                break;

            case StepMissionType.MINIGAME:
                string minigameId = questId + "_" + questStep.stepId;
                //find minigame in minigame map
                MinigameDataSO minigameData = minigameDataMap[minigameId];
                minigameData.Init(gameObject);
                break;

        }
    }

    public void OnQuestMinigameSuccess()
    {
        FinishQuestStep();
    }

    public void OnQuestMinigameFail()
    {
        StopInteract();
    }

    #endregion

    #region Quest Step Handle
    public void FinishQuestStep()
    {
        StopInteract();
        ResetNPC();
        questManager.OnFinishQuestStep(questId);
    }

    public void FinishQuestStepThenStartMinigame()
    {
        FinishQuestStep();
        StartQuestMinigame();
    }

    public void ResetNPC()
    {
        ChangeNPCState(NPCState.NORMAL);
        UpdateConservation();
    }

    public void UpdateQuestStep(QuestStep step, string questId)
    {
        this.questId = questId;
        questStep = step;
        ChangeNPCState(step.npcStatus);
        UpdateConservation();
    }

    public void UpdateConservation()
    {
        //update current conservation
        if (currentNPCState == NPCState.NORMAL)
        {
            currentDialogConservation = npcConservationMap[currentNPCState.ToString()];
        }
        else
        {
            string questStepConservationId = GetQuestStepConservationId();
            currentDialogConservation = npcConservationMap[questStepConservationId];
        }
    }

    public string GetQuestStepConservationId()
    {
        return currentNPCState
                + (questId != "" ? ("_" + questId) : "")
                + (questStep.stepId != "" ? ("_" + questStep.stepId) : "");
    }

    public void ChangeNPCState(NPCState state)
    {
        currentNPCState = state;
        //update state icon
        HideAllStateIcon();
        switch (currentNPCState)
        {
            case NPCState.NORMAL:
                if(iconDefault != null)
                {
                    iconDefault.SetActive(true);
                }
                break;

            case NPCState.HAVE_QUEST:
                iconQuestAvailable.SetActive(true);
                break;

            case NPCState.IN_PROGRESS:
                iconQuestInProgress.SetActive(true);
                break;

            case NPCState.QUEST_COMPLETE:
                iconQuestComplete.SetActive(true);
                break;

            case NPCState.COUNTDOWN:
                break;
        }
    }

    private void HideAllStateIcon()
    {
        iconQuestAvailable.SetActive(false);
        iconQuestComplete.SetActive(false);
        iconQuestInProgress.SetActive(false);
    }
    #endregion
}
