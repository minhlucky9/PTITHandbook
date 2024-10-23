using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalResponseData_Login;
public class QuestManager : MonoBehaviour
{
    public int FirstTimeQuest;
    public GameObject QR;
    public GameObject Turtorial;
    public bool IsQR = false;
    public int Medal { get; private set; } // Biến Medal sẽ lưu số lượng huân chương

    [Header("MovementFreezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;
    public MouseManager MouseManager;
    public ButtonActivator ButtonActivator;

    public static QuestManager instance { get; private set; }
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    public Dictionary<string, Quest> questMap;

    private Dictionary<string, float> failedQuestRetryTimes = new Dictionary<string, float>();

    // quest start requirements
    private int currentPlayerLevel;

    private void Awake()
    {
        FirstTimeQuest = GlobalResponseData.FirstTimeQuest;
        Debug.Log(FirstTimeQuest);
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the QuestManager across different scenes
        }
        else
        {
            Destroy(gameObject); // Destroys the duplicate instance if one already exists
            return;
        }
        if ( FirstTimeQuest == 0)
        {
            questMap = CreateQuestMap();
        }
        else
        {
            questMap = GlobalResponseData.quests;
            // Scan all quests in Resources and check if any are missing in questMap
            QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

            foreach (QuestInfoSO questInfo in allQuests)
            {
                if (!questMap.ContainsKey(questInfo.id))
                {
                    // If questMap doesn't contain this quest, add it
                    Debug.Log("Adding missing quest to questMap: " + questInfo.id);
                    questMap.Add(questInfo.id, LoadQuest(questInfo));
                }
            }
        }
      
    }

   
    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;
        GameEventsManager.instance.questEvents.onCancelQuest += CancelQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
        GameEventsManager.instance.questEvents.onCancelQuest -= CancelQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        if(FirstTimeQuest == 0)
        {
            Turtorial.SetActive(true);
            MouseManager.ShowCursor();
            ButtonActivator.IsUIShow = true;
            if (character.index == 0)
            {
                MovementToggle.isCheck = false;
                CameraToggle.isCheck = false;
            }
            else
            {
                MovementToggle2.isCheck = false;
                CameraToggle2.isCheck = false;
            }
        }

     

        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
       
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check player level requirements
        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        CheckFinishedQuestsForMedals();
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }

            // Check for retry times for failed quests
            foreach (var questRetryTime in failedQuestRetryTimes)
            {
                if (Time.time >= questRetryTime.Value)
                {
                    // Allow the quest to be started again
                    Quest QuestAgain = GetQuestById(questRetryTime.Key);
                    if (quest.state == QuestState.FAILED)
                    {
                        ChangeQuestState(quest.info.id, QuestState.CAN_START);
                    }
                }
            }
        }
    }

    private void CancelQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ChangeQuestState(id, QuestState.FAILED);

        // Set the retry time for 15 minutes later
        if (failedQuestRetryTimes.ContainsKey(id))
        {
            failedQuestRetryTimes[id] = Time.time + 16; // 15 minutes in seconds
        }
        else
        {
            failedQuestRetryTimes.Add(id, Time.time + 16);
        }
        Debug.Log("Quest failed");
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);

        // Remove from failed quest retry times if it was previously failed
        if (failedQuestRetryTimes.ContainsKey(id))
        {
            failedQuestRetryTimes.Remove(id);
        }
    }

    private void StartQuest(string id) 
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
        Debug.Log("Stat");
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        // move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    /*
    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }
    */

    private void ClaimRewards(Quest quest)
    {
        GameEventsManager.instance.goldEvents.GoldGained(quest.info.goldReward);
        GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
        Debug.Log("DCM");
    }

    private void SaveQuest(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();
            // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
            string serializedData = JsonUtility.ToJson(questData);
            Debug.Log(serializedData);
            // saving to PlayerPrefs is just a quick example for this tutorial video,
            // you probably don't want to save this info there long-term.
            // instead, use an actual Save & Load system and write to a file, the cloud, etc..
            PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try 
        {
            // load quest from saved data
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else 
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }


    public void CheckFinishedQuestsForMedals()
    {
        Medal = 0; // Reset Medal trước khi kiểm tra

        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.FINISHED)
            {
                Medal++; // Tăng số huân chương nếu quest đã hoàn thành
            }
        }
       

        Debug.Log("Total Medals: " + Medal);
    }

    public void CheckQR()
    {
        if (Medal == 10)
        {
            if (!IsQR)
            {
                MouseManager.ShowCursor();
                ButtonActivator.IsUIShow = true;
                if (character.index == 0)
                {
                    MovementToggle.isCheck = false;
                    CameraToggle.isCheck = false;
                }
                else
                {
                    MovementToggle2.isCheck = false;
                    CameraToggle2.isCheck = false;
                }
                QR.SetActive(true);
                IsQR = true;

               
            }
        }
    }
}
