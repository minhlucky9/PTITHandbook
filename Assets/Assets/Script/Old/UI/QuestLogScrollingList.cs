using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestLogScrollingList : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;

    [Header("Rect Transforms")]
    [SerializeField] private RectTransform scrollRectTransform;
    [SerializeField] private RectTransform contentRectTransform;

    [Header("Quest Log Button")]
    [SerializeField] private GameObject questLogButtonPrefab;

    private Dictionary<string, QuestLogButton> idToButtonMap = new Dictionary<string, QuestLogButton>();

    public QuestLogButton CreateButtonIfNotExists(Quest quest, UnityAction selectAction) 
    {
        QuestLogButton questLogButton = null;
        if (!idToButtonMap.ContainsKey(quest.info.id))
        {
            questLogButton = InstantiateQuestLogButton(quest, selectAction);
        }
        else 
        {
            questLogButton = idToButtonMap[quest.info.id];
            questLogButton.SetState(quest);  // Update the button state if it already exists
        }
        return questLogButton;
    }

    private QuestLogButton InstantiateQuestLogButton(Quest quest, UnityAction selectAction)
    {
        QuestLogButton questLogButton = Instantiate(
            questLogButtonPrefab,
            contentParent.transform).GetComponent<QuestLogButton>();

        questLogButton.gameObject.name = quest.info.id + "_button";

        RectTransform buttonRectTransform = questLogButton.GetComponent<RectTransform>();
        InitializeQuestLogButton(questLogButton, quest, selectAction, buttonRectTransform);

        idToButtonMap[quest.info.id] = questLogButton;
        return questLogButton;
    }

    private void InitializeQuestLogButton(QuestLogButton questLogButton, Quest quest, UnityAction selectAction, RectTransform buttonRectTransform)
    {
        questLogButton.Initialize(
            quest.info.displayName,
            quest.GetFullStatusText(),
            quest.info.goldReward + " Gold",
            () => {
                selectAction();
                UpdateScrolling(buttonRectTransform);
            }
        );
    }

    private void UpdateScrolling(RectTransform buttonRectTransform)
    {
        float buttonYMin = Mathf.Abs(buttonRectTransform.anchoredPosition.y);
        float buttonYMax = buttonYMin + buttonRectTransform.rect.height;

        float contentYMin = contentRectTransform.anchoredPosition.y;
        float contentYMax = contentYMin + scrollRectTransform.rect.height;

        if (buttonYMax > contentYMax)
        {
            contentRectTransform.anchoredPosition = new Vector2(
                contentRectTransform.anchoredPosition.x,
                buttonYMax - scrollRectTransform.rect.height
            );
        }
        else if (buttonYMin < contentYMin) 
        {
            contentRectTransform.anchoredPosition = new Vector2(
                contentRectTransform.anchoredPosition.x,
                buttonYMin
            );
        }
    }
}