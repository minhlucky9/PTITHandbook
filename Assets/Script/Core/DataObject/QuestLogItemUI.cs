using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Interaction;    // để dùng Quest, QuestState
using System;

public class QuestLogItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image questImage;
    [SerializeField] private GameObject requirementNotMetUI;
    [SerializeField] private Transform stateContainer; // chứa các icon/status child

    private Quest boundQuest;
    private Action onStateChangedHandler;

    public void Bind(Quest quest)
    {
        Unbind();

        boundQuest = quest;

        // 1. Text + description (clamp index)
        nameText.text = boundQuest.info.displayName;
        UpdateDescription();

        // 2. Ảnh
        questImage.sprite = boundQuest.info.questIcon;

        // 3. REQUIREMENTS_NOT_MET
        requirementNotMetUI.SetActive(boundQuest.state == QuestState.REQUIREMENTS_NOT_MET);

        // 4. Hiển thị state icon đúng
        UpdateStateIcons(boundQuest.state);

        // 5. Đăng ký lắng nghe khi state thay đổi
        onStateChangedHandler = () =>
        {
            UpdateDescription();
            requirementNotMetUI.SetActive(boundQuest.state == QuestState.REQUIREMENTS_NOT_MET);
            UpdateStateIcons(boundQuest.state);
        };
        boundQuest.OnStateChanged += onStateChangedHandler;
    }

    private void UpdateDescription()
    {
        int count = boundQuest.info.questSteps.Count;
        if (count > 0)
        {
            int idx = Mathf.Clamp(boundQuest.currentQuestStepIndex, 0, count - 1);
            descText.text = boundQuest.info.questSteps[idx].stepDescription;
        }
        else
        {
            descText.text = "Nhiệm vụ đã hoàn thành";
        }
    }

    private void UpdateStateIcons(QuestState state)
    {
        int idx = (int)state;
        for (int i = 0; i < stateContainer.childCount; i++)
        {
            stateContainer.GetChild(i).gameObject.SetActive(i == idx);
        }
    }

    private void Unbind()
    {
        if (boundQuest != null && onStateChangedHandler != null)
        {
            boundQuest.OnStateChanged -= onStateChangedHandler;
            onStateChangedHandler = null;
        }
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void OnDestroy()
    {
        Unbind();
    }
}
