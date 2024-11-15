using GameManager;
using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : TalkInteraction
{
    [Header("NPC infos")]
    public NPCInfoSO npcInfo;
    public NPCState currentNPCState;
    Dictionary<string, NPCConservationSO> npcConservationMap;
    Dictionary<string, QuizConservationSO> quizConservationMap;

    [HideInInspector] 
    public NPCConservationSO currentDialogConservation;
    QuestManager questManager;
    [Header("Quest Mission")]
    string questId;
    QuestStep questStep;
    QuizManager quizManager;

    [Header("State Symbol")]
    public GameObject iconQuestAvailable;
    public GameObject iconQuestComplete;
    public GameObject iconQuestInProgress;

    public override void Awake()
    {
        base.Awake();
        quizManager = FindObjectOfType<QuizManager>();
        questManager = FindAnyObjectByType<QuestManager>();
        //create data map
        npcConservationMap = CreateNPCConservationMap();
        quizConservationMap = CreateQuizConservationMap();
    }

    public override void Interact()
    {
        conservationManager.InitConservation(gameObject, currentDialogConservation);
        base.Interact();
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

    private Dictionary<string, QuizConservationSO> CreateQuizConservationMap()
    {
        // loads all QuestInfo Scriptable Objects under the Assets/Resources/Quests folder
        QuizConservationSO[] allConservations = Resources.LoadAll<QuizConservationSO>("NPC/" + npcInfo.npcId);

        // Create the quest map
        Dictionary<string, QuizConservationSO> npcConservationMap = new Dictionary<string, QuizConservationSO>();
        foreach (QuizConservationSO conservation in allConservations)
        {
            if (npcConservationMap.ContainsKey(conservation.conservationId))
            {
                Debug.LogWarning("Duplicate ID found when creating npc conservation map: " + conservation.conservationId);
            }
            npcConservationMap.Add(conservation.conservationId, conservation);
        }
        return npcConservationMap;
    }
    #endregion

    #region Quest Minigame Handle
    public void StartQuestMinigame()
    {
        switch (questStep.missionType)
        {
            case StepMissionType.TALKING:
                break;
            case StepMissionType.QUIZ:
                string quizId = questId + "_" + questStep.stepId;
                //find quiz in quiz map
                QuizConservationSO quizData = quizConservationMap[quizId];
                quizManager.InitAndStartQuizData(gameObject, quizData);
                break;
            case StepMissionType.MAZE:
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
