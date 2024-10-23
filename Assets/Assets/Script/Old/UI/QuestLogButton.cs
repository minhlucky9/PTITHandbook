using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class QuestLogButton : MonoBehaviour, ISelectHandler
{
    public Button button { get; private set; }
    private TextMeshProUGUI buttonText;
    public TextMeshProUGUI questStatusText;
    private TextMeshProUGUI goldRewardsText;
    private UnityAction onSelectAction;

    public void Initialize(string displayName, string status, string goldReward, UnityAction selectAction)
    {
        this.button = this.GetComponent<Button>();
        this.buttonText = this.transform.Find("QuestNameText").GetComponent<TextMeshProUGUI>();
        this.questStatusText = this.transform.Find("QuestStatusText").GetComponent<TextMeshProUGUI>();
        this.goldRewardsText = this.transform.Find("GoldRewardsText").GetComponent<TextMeshProUGUI>();

        this.buttonText.text = displayName;
        this.questStatusText.text = status;
        this.goldRewardsText.text = goldReward;
        this.onSelectAction = selectAction;
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectAction();
    }

    public void SetState(Quest quest)
    {
        Debug.Log("TTTT");
        // Update text based on quest state
        questStatusText.text = quest.GetFullStatusText();
        goldRewardsText.text = quest.info.goldReward + " Gold";

        // Change text color based on the quest state
        switch (quest.state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
            case QuestState.CAN_START:
                buttonText.color = Color.red;
                break;
            case QuestState.IN_PROGRESS:
                buttonText.color = Color.yellow;
                break;
            case QuestState.CAN_FINISH:
                buttonText.color = Color.yellow;
                break;
            case QuestState.FINISHED:
                buttonText.color = Color.green;
                break;
            default:
                Debug.LogWarning("Quest State not recognized by switch statement: " + quest.state);
                break;
        }
    }
}