using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Interaction;    // để dùng Quest, QuestState

public class QuestLogItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image questImage;
    [SerializeField] private GameObject requirementNotMetUI;
    [SerializeField] private Transform stateContainer; // chứa các icon/status child

    private Quest boundQuest;

    public void Bind(Quest quest)
    {
        boundQuest = quest;

        // 1. Text + description
        nameText.text = quest.info.displayName;
        descText.text = quest.info.questSteps[quest.currentQuestStepIndex].stepDescription;

        // 2. Ảnh
        questImage.sprite = quest.info.questIcon;

        // 3. REQUIREMENTS_NOT_MET
        requirementNotMetUI.SetActive(quest.state == QuestState.REQUIREMENTS_NOT_MET);

        // 4. Hiển thị state icon đúng
        UpdateStateIcons(quest.state);

        // 5. Đăng ký lắng nghe khi state thay đổi
        quest.OnStateChanged += () =>
        {
            // mỗi khi state đổi, cập nhật lại description & icon
            descText.text = quest.info.questSteps[quest.currentQuestStepIndex].stepDescription;
            requirementNotMetUI.SetActive(quest.state == QuestState.REQUIREMENTS_NOT_MET);
            UpdateStateIcons(quest.state);
            QuestLogManager.instance.UpdateCompletionCount();
        };
    }

    private void UpdateStateIcons(QuestState state)
    {
        int idx = (int)state;
        for (int i = 0; i < stateContainer.childCount; i++)
        {
            stateContainer.GetChild(i).gameObject.SetActive(i == idx);
        }
    }
}
