using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JamesInteraction_ShowMarket : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForJames;

    [Header("UI")]
    [SerializeField] private GameObject InteractUI;
    [SerializeField] private GameObject CarsUI;
    [SerializeField] private GameObject questStartUI;
    [SerializeField] private GameObject questProgressUI;
    [SerializeField] private GameObject questCompleteUI;
    [SerializeField] private Button startQuestButton;
    [SerializeField] private Button completeQuestButton;
    private bool playerIsNear = false;
   


    private bool questStarted = false;
    private bool questCompleted = false;

    private void Awake()
    {
       // questStartUI.SetActive(false);
        questCompleteUI.SetActive(false);

        startQuestButton.onClick.AddListener(StartQuest);
        completeQuestButton.onClick.AddListener(CompleteQuest);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed += OnSubmitPressed;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed -= OnSubmitPressed;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id == questInfoForJames.id)
        {
            if (quest.state == QuestState.CAN_FINISH)
            {
                questCompleted = true;
            }
            else if (quest.state == QuestState.FINISHED)
            {
                questCompleted = false ;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    private void OnSubmitPressed()
    {
        if (playerIsNear)
        {
            if (questCompleted)
            {
                questCompleteUI.SetActive(true);
            }
            else
            {
                questStartUI.SetActive(true);
            }
        }
    }

    private void OnQuestFinished(string questId)
    {
        // Assuming this is the specific quest ID you want to track
        if (questId == "CollectCoinsQuest")
        {
            questCompleted = true;
        }
    }

    private void StartQuest()
    {
     //   questStarted = true;
        InteractUI.SetActive(false);
        questStartUI.SetActive(false);
        questProgressUI.SetActive(true);
        questCompleteUI.SetActive(true);
       CarsUI.SetActive(false);
        GameEventsManager.instance.questEvents.StartQuest(questInfoForJames.id);
    }

    private void CompleteQuest()
    {
        if (questCompleted)
        {
         //   questStartUI.SetActive(false);
           questCompleteUI.SetActive(false);
            GameEventsManager.instance.questEvents.FinishQuest(questInfoForJames.id);
            questProgressUI.SetActive(false);
        }
      
       // questCompleteUI.SetActive(false);



    }
}
