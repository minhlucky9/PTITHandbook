using UnityEngine;
using GameManager; // Gi? s? QuestManager v� Quest n?m trong namespace GameManager

// N?u b?n ?� t?o l?p QuizQuest k? th?a t? Quest, h�y ??m b?o namespace c?a n� ???c import
// V� d?: using YourNamespace; 

public class QuestProgressViewer : MonoBehaviour
{
    // C�c th�ng s? hi?n th?
    public int startX = 10;
    public int startY = 10;
    public int lineHeight = 30;

    void OnGUI()
    {
        // Ki?m tra QuestManager ?� kh?i t?o ch?a
        if (QuestManager.instance == null)
        {
            GUI.Label(new Rect(startX, startY, 300, lineHeight), "QuestManager ch?a ???c kh?i t?o.");
            return;
        }

        // L?y questMap t? QuestManager
        var questMap = QuestManager.instance.questMap;
        if (questMap == null || questMap.Count == 0)
        {
            GUI.Label(new Rect(startX, startY, 300, lineHeight), "Kh�ng c� quest n�o ???c load.");
            return;
        }

        int currentY = startY;
        GUI.Label(new Rect(startX, currentY, 600, lineHeight), "=== Quest Progress Viewer ===");
        currentY += lineHeight;

        // Duy?t qua t?ng quest trong questMap
        foreach (var kvp in questMap)
        {
            Quest quest = kvp.Value;
            // Hi?n th? th�ng tin c? b?n: QuestID, tr?ng th�i, b??c hi?n t?i v� t?ng s? b??c
            string questInfo = string.Format("Quest ID: {0} | State: {1} | Step: {2}/{3}",
                                             quest.info.id, quest.state.ToString(),
                                             quest.currentQuestStepIndex, quest.info.questSteps.Count);

            // N?u quest l� QuizQuest (l?p con c?a Quest d�ng cho quiz), �p ki?u v� hi?n th? th�m th�ng tin quiz
            QuizQuest quizQuest = quest as QuizQuest;
            if (quizQuest != null)
            {
                questInfo += string.Format(" | Quiz: Q={0} Correct={1} Wrong={2}",
                                             quizQuest.currentQuestion, quizQuest.correctAnswers, quizQuest.wrongAnswers);
            }

            GUI.Label(new Rect(startX, currentY, 600, lineHeight), questInfo);
            currentY += lineHeight;
        }
    }
}
