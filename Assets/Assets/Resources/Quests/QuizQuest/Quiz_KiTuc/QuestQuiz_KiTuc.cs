
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestQuiz_KiTuc : QuestStep
{
    private int correctAnswersRequired = 7;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (Quiz_KiTuc.instance != null)
        {
            Quiz_KiTuc.instance.OnAnswerSelected += CheckQuestCompletion;
            Quiz_KiTuc.instance.OnWrongAnswer += CheckQuestCancellation;
        }
    }

    private void OnDisable()
    {
        if (Quiz_KiTuc.instance != null)
        {
            Quiz_KiTuc.instance.OnAnswerSelected -= CheckQuestCompletion;
            Quiz_KiTuc.instance.OnWrongAnswer -= CheckQuestCancellation;
        }
    }

    private void CheckQuestCompletion()
    {
        if (Quiz_KiTuc.instance.GetScore() >= correctAnswersRequired)
        {
            FinishQuestStep();
            Quiz_KiTuc.instance.GameOver();
        }
        UpdateState();
    }

    private void CheckQuestCancellation()
    {
        if (Quiz_KiTuc.instance.GetWrongScore() >= 3)
        {
            CancelQuestStep();
        }
    }

    private void UpdateState()
    {
        if (Quiz_KiTuc.instance != null)
        {
            int currentScore = Quiz_KiTuc.instance.GetScore();
            string state = currentScore.ToString();
            string status = "Correct Answers: " + currentScore + " / " + correctAnswersRequired;
            ChangeState(state, status);
        }
    }

    protected override void SetQuestStepState(string state)
    {
        int savedScore;
        /*
        if (int.TryParse(state, out savedScore) && QuizA1.instance != null)
        {
            // Assuming we want to directly set the score in the QuizManager for some reason
            while (QuizA1.instance.GetScore() < savedScore)
            {
                QuizA1.instance.AnswerSelected(true, null); // Manually incrementing score for restoration
            }
            UpdateState();
        }
        */
    }

    private void CancelQuestStep()
    {
      
        Debug.Log("Quest canceled due to too many wrong answers.");
        // Add any additional logic needed for quest cancellation
    }
}
