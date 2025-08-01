﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameManager;    // n?u QuestManager n?m namespace GameManager
using Interaction;    // ?? dùng Quest, QuestState

public class QuestLogManager : MonoBehaviour
{
    public static QuestLogManager instance;

    [Header("Quest Order")]
    [SerializeField] private List<QuestInfoSO> basicQuestSOList;
    [SerializeField] private List<QuestInfoSO> advancedQuestSOList;

    [Header("Prefabs & UI References")]
    [SerializeField] private GameObject questItemPrefab;   // Prefab c?a ô Quest
    [SerializeField] private Transform contentContainer;   // GameObject ch?a VerticalLayoutGroup
    [SerializeField] private Button basicButton;
    [SerializeField] private Button advancedButton;

    [Header("Completion Counter UI")]
    [SerializeField] private TextMeshProUGUI completionText;

    private List<Quest> basicQuests;
    private List<Quest> advancedQuests;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        basicButton.onClick.AddListener(() => ShowQuests(basicQuestSOList));
        advancedButton.onClick.AddListener(() => ShowQuests(advancedQuestSOList));
        ShowQuests(basicQuestSOList);
        UpdateBasicQuest();
    }
/*
    private void OnEnable()
    {
        QuestManager.instance.OnQuestsInitialized += InitiateQuestLog;
    }

    private void OnDisable()
    {
        QuestManager.instance.OnQuestsInitialized -= InitiateQuestLog;
    }

    public void InitiateQuestLog()
    {
        var allQuests = QuestManager.instance.questMap.Values;


        basicQuests = new List<Quest>();
        advancedQuests = new List<Quest>();
        foreach (var q in allQuests)
        {
            if (q.info.isAdvancedQuest) advancedQuests.Add(q);
            else basicQuests.Add(q);
        }

        // 3. Thêm listener cho button
        basicButton.onClick.AddListener(() => ShowQuests(basicQuests));
        advancedButton.onClick.AddListener(() => ShowQuests(advancedQuests));

        ShowQuests(basicQuests);
    }
*/
    private void ShowQuests(List<QuestInfoSO> soList)
    {
        // 1. Xoá UI cũ
        foreach (Transform c in contentContainer) Destroy(c.gameObject);

        // 2. Tạo theo đúng thứ tự trong soList
        foreach (var so in soList)
        {
            // Lấy Quest từ QuestManager
            if (!QuestManager.instance.questMap.TryGetValue(so.id, out var quest)) continue;
            var go = Instantiate(questItemPrefab, contentContainer);
            go.GetComponent<QuestLogItemUI>().Bind(quest);
        }
    }

    private void UpdateCompletionCount(List<QuestInfoSO> soList)
    {
        int total = soList.Count;
        int done = 0;
        foreach (var so in soList)
        {
            if (QuestManager.instance.questMap.TryGetValue(so.id, out var q)
             && q.state == QuestState.FINISHED)  
            {
                done++;
            }
        }
        completionText.text = $"Hoàn thành {done}/{total}";
    }

    public void UpdateBasicQuest()
    {
        UpdateCompletionCount(basicQuestSOList);
  
    }

    public void UpdateAdvancedQuest()
    {
        UpdateCompletionCount(advancedQuestSOList);
    }

    public void OpenQuestLog()
    {
        ShowQuests(basicQuestSOList);
    }
}
