using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using static GlobalResponseData_Login;
using Inventory.Model;
using Newtonsoft.Json; // Include your namespace for ItemSO and InventoryItem

public class PlayerDataLoader : MonoBehaviour
{
    public TMP_InputField outputArea;

    private string apiUrl = "http://1.55.212.49:8098/DemoBackend3D_API/player/getLastStatePlayer";

    void Start()
    {
        if (outputArea == null)
        {
            outputArea = GameObject.Find("OutputArea").GetComponent<TMP_InputField>();
        }
    }

    public void LoadPlayerData()
    {
        StartCoroutine(LoadPlayerData_Coroutine());
    }

    IEnumerator LoadPlayerData_Coroutine()
    {
        outputArea.text = "Loading player data...";

        SessionData sessionData = new SessionData
        {
            session_id = GlobalResponseData.session_id
        };

        string jsonData = JsonUtility.ToJson(sessionData);

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
                outputArea.text = "Player data loaded successfully!";   
                string json = JsonConvert.SerializeObject(responseData.player_state);
                PlayerState playerState = responseData.player_state;
                    Debug.Log(json + "  1");
                if (playerState.last_state.level == 0)
                {
                    GlobalResponseData.gold = 200;
                    GlobalResponseData.x = 20.799999237060547f;
                    GlobalResponseData.y = 0.7609999775886536f;
                    GlobalResponseData.z = 69.4000015258789f;
                    GlobalResponseData.level = 1;
                    GlobalResponseData.HealthSlider = 100;
                    GlobalResponseData.FirstTimeQuest = 0;
                }
                else
                {
                    // Update player position, score, level, and FirstTimeQuest
                    GlobalResponseData.gold = playerState.score;
                    GlobalResponseData.x = playerState.last_state.x;
                    GlobalResponseData.y = playerState.last_state.y;
                    GlobalResponseData.z = playerState.last_state.z;
                    GlobalResponseData.level = playerState.last_state.level;
                    GlobalResponseData.HealthSlider = playerState.last_state.HealthSlider;
                    GlobalResponseData.FirstTimeQuest = playerState.last_state.FirstTimeQuest;
                    GlobalResponseData.Medal = playerState.last_state.Medal;    

                    // Convert quests from List to Dictionary
                    Dictionary<string, Quest> questMap = new Dictionary<string, Quest>();
                    foreach (QuestData questData in playerState.last_state.questMap.quests)
                    {
                        QuestInfoSO questInfoSO = FindQuestInfoById(questData.id);

                        if (questInfoSO != null)
                        {
                            Quest quest = new Quest(questInfoSO, questData.state, questData.questStepIndex, questData.stepStates);
                            questMap.Add(quest.info.id, quest);
                        }
                    }
                    GlobalResponseData.quests = questMap;

                    // Load player inventory from server data
                    List<InventoryItem> loadedInventory = new List<InventoryItem>();
                    foreach (var itemData in playerState.last_state.inventory.items)
                    {
                        ItemSO itemSO = FindItemSOByName(itemData.itemName);
                        if (itemSO != null)
                        {
                            InventoryItem inventoryItem = new InventoryItem
                            {
                                item = itemSO,
                                quantity = itemData.quantity
                            };
                            loadedInventory.Add(inventoryItem);
                        }
                        else
                        {
                            Debug.LogWarning("ItemSO with name " + itemData.itemName + " not found.");
                        }
                    }
                    GlobalResponseData.inventoryItems = loadedInventory; // Assuming you store the inventory in GlobalResponseData
                }
       
            }
            else
            {
                outputArea.text = $"Error {responseData.code}: {responseData.desc}";
            }
        }
    }

    private QuestInfoSO FindQuestInfoById(string id)
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (questInfo.id == id)
            {
                return questInfo;
            }
        }

        Debug.LogWarning("QuestInfoSO with ID " + id + " not found.");
        return null;
    }

    private ItemSO FindItemSOByName(string itemName)
    {
        ItemSO[] allItems = Resources.LoadAll<ItemSO>("Items"); // Assuming all items are in the "Items" folder

        foreach (ItemSO item in allItems)
        {
            if (item.Name == itemName)
            {
                return item;
            }
        }

        Debug.LogWarning("ItemSO with name " + itemName + " not found.");
        return null;
    }
    [System.Serializable]
    public class SessionData
    {
        public string session_id;
    }

    [System.Serializable]
    public class ResponseData
    {
        public int code;
        public string desc;
        public PlayerState player_state;
    }

    [System.Serializable]
    public class PlayerState
    {
        public int ID;
        public PlayerLastState last_state;
        public int score;
        public string last_login;
    }

    [System.Serializable]
    public class PlayerLastState
    {
        public float x;
        public float y;
        public float z;
        public int level;
        public int FirstTimeQuest;
        public float HealthSlider;
        public int Medal;
        public QuestMap questMap;
        public Inventory inventory;
    }

    [System.Serializable]
    public class QuestMap
    {
        public List<QuestData> quests = new List<QuestData>();
    }

    [System.Serializable]
    public class QuestData
    {
        public string id;
        public QuestState state;
        public int questStepIndex;
        public QuestStepState[] stepStates;
    }

    [System.Serializable]
    public class Inventory
    {
        public List<InventoryItemData> items = new List<InventoryItemData>();
    }

    [System.Serializable]
    public class InventoryItemData
    {
        public string itemName;
        public int quantity;
    }
}
