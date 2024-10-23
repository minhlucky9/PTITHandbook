using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Inventory.Model;
using static GlobalResponseData_Login;
using UnityEngine.UI;

public class PlayerDataManager : MonoBehaviour
{
    public TMP_InputField outputArea;
    public GoldManager score;
    public GameObject player;
    public PlayerLevelManager level;
    public InventorySO playerInventory;
    public int FirstTimeQuest = 1;
    public Slider HealthSlider;
    public QuestManager QuestManager;

    private string apiUrl = "http://1.55.212.49:8098/DemoBackend3D_API/player/updateLastStatePlayer";

    void Start()
    {
        if (outputArea == null)
        {
            outputArea = GameObject.Find("OutputArea").GetComponent<TMP_InputField>();
        }
    }

    public void fomo()
    {
        Vector3 position = player.transform.position;
        PostPlayerData(score.currentGold, position.x, position.y, position.z, level.currentLevel, FirstTimeQuest, QuestManager.instance.questMap, playerInventory.GetCurrentInventoryState(), HealthSlider.value, QuestManager.instance.Medal, GlobalResponseData.student_id, GlobalResponseData.fullname);
    }

    public void PostPlayerData(int score, float x, float y, float z, int level, int FirstTimeQuest, Dictionary<string, Quest> questMap, Dictionary<int, InventoryItem> inventoryMap, float HealthSlider, int Medal, string student_id, string fullname)
    {
        StartCoroutine(PostPlayerData_Coroutine(score, x, y, z, level, FirstTimeQuest, questMap, inventoryMap, HealthSlider, Medal, student_id, fullname));
    }

    IEnumerator PostPlayerData_Coroutine(int score, float x, float y, float z, int level, int FirstTimeQuest, Dictionary<string, Quest> questMap, Dictionary<int, InventoryItem> inventoryMap, float HealthSlider, int Medal, string student_id, string fullname)
    {
        outputArea.text = "Updating player data...";

        PlayerData playerData = new PlayerData
        {
            session_id = GlobalResponseData.session_id,
            details = new PlayerDetails(score, x, y, z, level, FirstTimeQuest, questMap, inventoryMap, HealthSlider, Medal, student_id, fullname)
        };

        string jsonData = JsonUtility.ToJson(playerData);
        Debug.Log(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            outputArea.text = request.error;
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

            if (responseData.code == 200)
            {
                outputArea.text = "Update Success!";
            }
            else if (responseData.code == 700)
            {
                outputArea.text = $"Error {responseData.code}: {responseData.desc}";
            }
            else
            {
                outputArea.text = $"Unexpected response code: {responseData.code}";
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string session_id;
        public PlayerDetails details;
    }

    [System.Serializable]
    public class PlayerDetails
    {
        public int score;
        public float x;
        public float y;
        public float z;
        public int level;
        public int FirstTimeQuest;
        public float HealthSlider;
        public int Medal;
        public string student_id;
        public string fullname;
        public SerializableQuestMap questMap;
        public SerializableInventory inventory;

        public PlayerDetails(int score, float x, float y, float z, int level, int FirstTimeQuest, Dictionary<string, Quest> questMap, Dictionary<int, InventoryItem> inventoryMap, float HealthSlider, int medal, string student_id, string fullname)
        {
            this.score = score;
            this.x = x;
            this.y = y;
            this.z = z;
            this.level = level;
            this.HealthSlider = HealthSlider;
            this.FirstTimeQuest = FirstTimeQuest;
            this.questMap = new SerializableQuestMap(questMap);
            this.inventory = new SerializableInventory(inventoryMap);
            this.Medal = medal;
            this.student_id = student_id;
            this.fullname = fullname;

        }
    }

    [System.Serializable]
    public class SerializableQuest
    {
        public string id;
        public QuestState state;
        public int currentStepIndex;
        public QuestStepState[] stepStates;

        public SerializableQuest(Quest quest)
        {
            this.id = quest.info.id;
            this.state = quest.state;
            this.currentStepIndex = quest.currentQuestStepIndex;
            this.stepStates = quest.questStepStates;
        }
    }

    [System.Serializable]
    public class SerializableQuestMap
    {
        public List<SerializableQuest> quests = new List<SerializableQuest>();

        public SerializableQuestMap(Dictionary<string, Quest> questMap)
        {
            foreach (var quest in questMap.Values)
            {
                quests.Add(new SerializableQuest(quest));
            }
        }
    }

    [System.Serializable]
    public class SerializableInventory
    {
        public List<SerializableInventoryItem> items = new List<SerializableInventoryItem>();

        public SerializableInventory(Dictionary<int, InventoryItem> inventoryMap)
        {
            foreach (var item in inventoryMap.Values)
            {
                items.Add(new SerializableInventoryItem(item));
            }
        }
    }

    [System.Serializable]
    public class SerializableInventoryItem
    {
        public int itemId;
        public string itemName;
        public int quantity;

        public SerializableInventoryItem(InventoryItem inventoryItem)
        {
            this.itemId = inventoryItem.item.ID;
            this.itemName = inventoryItem.item.Name;
            this.quantity = inventoryItem.quantity;
        }
    }

    [System.Serializable]
    public class ResponseData
    {
        public int code;
        public string desc;
    }
}
