using Interaction;
using PlayerStatsController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager instance;
        Dictionary<string, NPCController> npcMap;
        [HideInInspector] public Dictionary<string, Quest> questMap;
        PlayerStats playerStats;
   
        [Header("Config")]
        [SerializeField] private bool loadQuestState = true;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            questMap = CreateQuestMap();
            npcMap = CreateNPCMap();
            playerStats = FindObjectOfType<PlayerStats>();
            UpdateRequirementsMetQuest();
        }

        #region Handle Quest Step Update

        public void UpdateRequirementsMetQuest()
        {
            // loop through ALL quests
            foreach (Quest quest in questMap.Values)
            {
                // if we're now meeting the requirements, switch over to the CAN_START state
                if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
                {
                    quest.ChangeQuestState(QuestState.CAN_START);
                    InitQuestStep(quest.info.questSteps[quest.currentQuestStepIndex], quest.info.id);
                }
            }
        }

        public void InitQuestStep(QuestStep questStep, string questId)
        {
            NPCController npc = GetNPCById(questStep.npcId);
            npc.UpdateQuestStep(questStep, questId);
        }

        public void OnFinishQuestStep(string questId)
        {
            bool isQuestFinished = !questMap[questId].TryNextStep();

            if (isQuestFinished)
            {
                Debug.Log("Quest complete..reward");
            } else
            {
                Quest quest = questMap[questId];
                InitQuestStep(quest.info.questSteps[quest.currentQuestStepIndex], quest.info.id);
            }
        }

        #endregion

        #region Handle Reward
        public int CheckFinishedQuestsForMedals()
        {
            int medal = 0; // Reset Medal trước khi kiểm tra

            foreach (Quest quest in questMap.Values)
            {
                if (quest.state == QuestState.FINISHED)
                {
                    medal++; // Tăng số huân chương nếu quest đã hoàn thành
                }
            }

            return medal;
        }

        #endregion

        #region Handle Quest Manager Data

        private bool CheckRequirementsMet(Quest quest)
        {
            // start true and prove to be false
            bool meetsRequirements = true;

            // check player level requirements
            if (playerStats.characterLevel < quest.info.levelRequirement)
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

        private Quest GetQuestById(string id)
        {
            Quest quest = questMap[id];
            if (quest == null)
            {
                Debug.LogError("ID not found in the Quest Map: " + id);
            }
            return quest;
        }

        private Dictionary<string, Quest> CreateQuestMap()
        {
            // loads all QuestInfo Scriptable Objects under the Assets/Resources/Quests folder
            QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
            
            // Create the quest map
            Dictionary<string, Quest> questMap = new Dictionary<string, Quest>();
            foreach (QuestInfoSO questInfo in allQuests)
            {
                if (questMap.ContainsKey(questInfo.id))
                {
                    Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
                }
                questMap.Add(questInfo.id, LoadQuest(questInfo));
            }
            return questMap;
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

        private Dictionary<string, NPCController> CreateNPCMap()
        {
            NPCController[] npcs = FindObjectsOfType<NPCController>();

            // Create the quest map
            Dictionary<string, NPCController> npcMap = new Dictionary<string, NPCController>();
            foreach (NPCController npc in npcs)
            {
                if (npcMap.ContainsKey(npc.npcInfo.npcId))
                {
                    Debug.LogWarning("Duplicate ID found when creating NPC map: " + npc.npcInfo.npcId);
                }
                npcMap.Add(npc.npcInfo.npcId, npc);
            }
            return npcMap;
        }

        private NPCController GetNPCById(string id)
        {
            NPCController npc = npcMap[id];
            if (npc == null)
            {
                Debug.LogError("ID not found in the NPC Map: " + id);
            }
            return npc;
        }

        #endregion
    }
}

